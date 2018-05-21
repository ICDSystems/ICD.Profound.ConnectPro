﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Presentation;
using ICD.Connect.Conferencing.Controls;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcSharePresenter : AbstractPresenter<IVtcShareView>, IVtcSharePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private ISource[] m_Sources;
		private ISource m_Selected;
		private ISource m_Routed;

		[CanBeNull]
		private PresentationComponent m_SubscribedPresentationComponent;

		[CanBeNull]
		private IRoutingGraph m_SubscribedRoutingGraph;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcSharePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_Sources = new ISource[0];
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcShareView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				m_Sources = GetSources().ToArray();

				for (ushort index = 0; index < m_Sources.Length; index++)
				{
					ISource source = m_Sources[index];
					ConnectProSource connectProSource = source as ConnectProSource;

					IRoom room = Room == null || source == null ? null : Room.Routing.GetRoomForSource(source);
					bool combine = room != null && room.CombineState;

					string icon =
						connectProSource == null
						? null
						: Icons.GetSourceIcon(connectProSource.Icon, eSourceColor.White);

					view.SetButtonSelected(index, source == (m_Routed ?? m_Selected));
					view.SetButtonIcon(index, icon);
					view.SetButtonLabel(index, source == null ? null : source.GetNameOrDeviceName(combine));
				}

				bool inPresentation = IsInPresentation();
				bool enabled = inPresentation || m_Selected != null || m_Routed != null;

				view.SetButtonCount((ushort)m_Sources.Length);
				view.SetShareButtonEnabled(enabled);
				view.SetShareButtonSelected(inPresentation);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Gets the sources available for presentation.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<ISource> GetSources()
		{
			return
				Room == null
					? Enumerable.Empty<ISource>()
					: Room.Routing
					      .GetCoreSources()
					      .Where(s =>
					             {
						             ConnectProSource source = s as ConnectProSource;
						             return source != null && source.Share;
					             });
		}

		/// <summary>
		/// Returns true if we are currently presenting a source.
		/// </summary>
		/// <returns></returns>
		private bool IsInPresentation()
		{
			return m_SubscribedPresentationComponent != null && m_SubscribedPresentationComponent.GetPresentations().Any();
		}

		private void SetSelected(ISource source)
		{
			if (source == m_Selected)
				return;

			m_Selected = source;

			RefreshIfVisible();
		}

		private void SetRouted(ISource source)
		{
			if (source == m_Routed)
				return;

			m_Routed = source;

			SetSelected(null);
			RefreshIfVisible();
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			if (room.Core.TryGetRoutingGraph(out m_SubscribedRoutingGraph))
				m_SubscribedRoutingGraph.OnRouteChanged += RoutingGraphOnRouteChanged;

			IDialingDeviceControl dialer = room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			CiscoCodec codec = dialer == null ? null : dialer.Parent as CiscoCodec;
			m_SubscribedPresentationComponent = codec == null ? null : codec.Components.GetComponent<PresentationComponent>();

			if (m_SubscribedPresentationComponent == null)
				return;

			m_SubscribedPresentationComponent.OnPresentationsChanged += SubscribedPresentationComponentOnPresentationsChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedRoutingGraph != null)
				m_SubscribedRoutingGraph.OnRouteChanged -= RoutingGraphOnRouteChanged;

			m_SubscribedRoutingGraph = null;

			if (m_SubscribedPresentationComponent == null)
				return;

			m_SubscribedPresentationComponent.OnPresentationsChanged -= SubscribedPresentationComponentOnPresentationsChanged;

			m_SubscribedPresentationComponent = null;
		}

		private void SubscribedPresentationComponentOnPresentationsChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private void RoutingGraphOnRouteChanged(object sender, SwitcherRouteChangeEventArgs eventArgs)
		{
			if (Room == null)
				return;

			// Update the routed presentation source
			ISource source = Room.Routing.GetVtcPresentationSource();
			SetRouted(source);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcShareView view)
		{
			base.Subscribe(view);

			view.OnSourceButtonPressed += ViewOnSourceButtonPressed;
			view.OnShareButtonPressed += ViewOnShareButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcShareView view)
		{
			base.Unsubscribe(view);

			view.OnSourceButtonPressed -= ViewOnSourceButtonPressed;
			view.OnShareButtonPressed -= ViewOnShareButtonPressed;
		}

		private void ViewOnShareButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null || m_Selected == null)
				return;

			if (IsInPresentation())
				StopPresenting();
			else
				StartPresenting(m_Selected);
		}

		private void ViewOnSourceButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			ISource source;
			if (!m_Sources.TryElementAt(eventArgs.Data, out source))
				return;

			if (IsInPresentation())
				StartPresenting(source);
			else
				SetSelected(source);
		}

		private void StartPresenting(ISource source)
		{
			if (Room == null || source == null)
				return;

			Room.Routing.RouteVtcPresentation(source);
		}

		private void StopPresenting()
		{
			if (m_SubscribedPresentationComponent == null)
				return;

			foreach (PresentationItem presentation in m_SubscribedPresentationComponent.GetPresentations())
				m_SubscribedPresentationComponent.StopPresentation(presentation);
		}

		#endregion
	}
}
