using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Sources;
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
		private readonly IcdHashSet<ISource> m_Routed;

		private IDialingDeviceControl m_SubscribedVideoDialer;

		[CanBeNull]
		private IPresentationControl m_SubscribedPresentationComponent;

		[CanBeNull]
		private IRoutingGraph m_SubscribedRoutingGraph;

		private ISource Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshIfVisible();
			}
		}

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
			m_Routed = new IcdHashSet<ISource>();
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

				bool inPresentation = IsInPresentation();
				
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

					bool select = m_Routed.Count == 0
						? source == m_Selected
						: m_Routed.Contains(source);

					view.SetButtonSelected(index, select);
					view.SetButtonIcon(index, icon);
					view.SetButtonLabel(index, source == null ? null : source.GetNameOrDeviceName(combine));
				}

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
			return m_SubscribedPresentationComponent != null && m_SubscribedPresentationComponent.PresentationActiveInput != null;
		}

		private void UpdatePresentationRoutedSources()
		{
			m_Routed.Clear();

			if (IsInPresentation())
			{
				// Update the routed presentation source
				IEnumerable<ISource> sources = Room == null
					? Enumerable.Empty<ISource>()
					: Room.Routing.GetVtcPresentationSources().ToIcdHashSet();

				m_Routed.AddRange(sources);

				// Always clear selection in presentation mode
				Selected = null;
			}

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

			m_SubscribedVideoDialer = room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			if (m_SubscribedVideoDialer != null)
			{
				m_SubscribedVideoDialer.OnSourceAdded += VideoDialerOnSourceAdded;
				m_SubscribedVideoDialer.OnSourceChanged += VideoDialerOnSourceChanged;
				m_SubscribedVideoDialer.OnSourceRemoved += VideoDialerOnSourceRemoved;
			}

			if (room.Core.TryGetRoutingGraph(out m_SubscribedRoutingGraph))
				m_SubscribedRoutingGraph.RoutingCache.OnEndpointRouteChanged += RoutingCacheOnEndpointRouteChanged;

			IDialingDeviceControl dialer = room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			IVideoConferenceDevice codec = dialer == null ? null : dialer.Parent as IVideoConferenceDevice;
			m_SubscribedPresentationComponent = codec == null ? null : codec.Controls.GetControl<IPresentationControl>();

			if (m_SubscribedPresentationComponent == null)
				return;

			m_SubscribedPresentationComponent.OnPresentationActiveInputChanged += SubscribedPresentationControlOnPresentationActiveInputChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedVideoDialer != null)
			{
				m_SubscribedVideoDialer.OnSourceAdded -= VideoDialerOnSourceAdded;
				m_SubscribedVideoDialer.OnSourceChanged -= VideoDialerOnSourceChanged;
				m_SubscribedVideoDialer.OnSourceRemoved -= VideoDialerOnSourceRemoved;
			}
			m_SubscribedVideoDialer = null;

			if (m_SubscribedRoutingGraph != null)
				m_SubscribedRoutingGraph.RoutingCache.OnEndpointRouteChanged -= RoutingCacheOnEndpointRouteChanged;

			m_SubscribedRoutingGraph = null;

			if (m_SubscribedPresentationComponent != null)
				m_SubscribedPresentationComponent.OnPresentationActiveInputChanged -= SubscribedPresentationControlOnPresentationActiveInputChanged;

			m_SubscribedPresentationComponent = null;
		}

		private void SubscribedPresentationControlOnPresentationActiveInputChanged(object sender, EventArgs eventArgs)
		{
			UpdatePresentationRoutedSources();
		}

		private void RoutingCacheOnEndpointRouteChanged(object sender, EndpointRouteChangedEventArgs endpointRouteChangedEventArgs)
		{
			UpdatePresentationRoutedSources();
		}

		private void VideoDialerOnSourceChanged(object sender, ConferenceSourceEventArgs eventArgs)
		{
			UpdateVisibility();
		}

		private void VideoDialerOnSourceRemoved(object sender, ConferenceSourceEventArgs eventArgs)
		{
			UpdateVisibility();
		}

		private void VideoDialerOnSourceAdded(object sender, ConferenceSourceEventArgs eventArgs)
		{
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			// Ensure we leave presentation mode when we leave a call
			if (m_SubscribedPresentationComponent != null)
				StopPresenting();

			bool isInCall = m_SubscribedVideoDialer != null && m_SubscribedVideoDialer.GetSources().Any(s => s.GetIsOnline());
			if (!isInCall)
				ShowView(false);
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
			if (Room == null)
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
				Selected = source;
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

			m_SubscribedPresentationComponent.StopPresentation();
		}

		#endregion
	}
}
