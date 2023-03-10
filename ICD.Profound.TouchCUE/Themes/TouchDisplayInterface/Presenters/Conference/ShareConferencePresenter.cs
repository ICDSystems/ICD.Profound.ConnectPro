using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Background;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IShareConferencePresenter))]
	public sealed class ShareConferencePresenter : AbstractConferencePresenter<IShareConferenceView>, IShareConferencePresenter
	{
		private const string SHARE_BUTTON_TEXT_NOT_SHARING = "START SHARING";
		private const string SHARE_BUTTON_TEXT_SHARING = "STOP SHARING";

		private readonly SafeCriticalSection m_RefreshSection;

		private readonly IcdHashSet<ISource> m_RoutedSources;

		private ISource[] m_Sources;
		private ISource m_Selected;

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

		public ShareConferencePresenter(INavigationController nav, IViewFactory views, TouchCueTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_RoutedSources = new IcdHashSet<ISource>();
			m_Sources = new ISource[0];
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IShareConferenceView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool inPresentation = IsInPresentation();

				for (ushort index = 0; index < m_Sources.Length; index++)
				{
					ISource source = m_Sources[index];

					IRoom room = Room == null || source == null ? null : Room.Routing.Sources.GetRoomForSource(source);
					bool combine = room != null && room.CombineState;

					string icon = TouchCueIcons.GetSourceIcon(source, eTouchCueColor.White);

					bool select = inPresentation ? m_RoutedSources.Contains(source) : source == m_Selected;

					view.SetButtonSelected(index, select);
					view.SetButtonVisible(index, true);
					view.SetButtonIcon(index, icon);
					view.SetButtonLabel(index, source == null ? null : source.GetName(combine));
				}

				bool enabled = inPresentation || m_Selected != null;

				view.SetButtonCount((ushort)m_Sources.Length);
				view.SetShareButtonEnabled(enabled);
				view.SetShareButtonSelected(inPresentation);
				view.SetShareButtonText(inPresentation ? SHARE_BUTTON_TEXT_SHARING : SHARE_BUTTON_TEXT_NOT_SHARING);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the room for the presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			UpdateSources();
		}

		private void UpdateSources()
		{
			m_Sources = GetSources().ToArray();
			RefreshIfVisible();
		}

		/// <summary>
		/// Gets the sources available for presentation.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<ISource> GetSources()
		{
			IVideoConferenceRouteControl routeControl = m_SubscribedPresentationComponent == null
				? null
				: m_SubscribedPresentationComponent.Parent.Controls.GetControl<IVideoConferenceRouteControl>();

			return
				Room == null || routeControl == null
					? Enumerable.Empty<ISource>()
					: Room.Routing
					      .Sources
					      .GetRoomSourcesForPresentation(routeControl);
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
			m_RoutedSources.Clear();

			if (IsInPresentation())
			{
				// Update the routed presentation source
				IEnumerable<ISource> sources = Room == null
					? Enumerable.Empty<ISource>()
					: Room.Routing.Sources.GetVtcPresentationSources(m_SubscribedPresentationComponent);

				m_RoutedSources.AddRange(sources);

				// Always clear selection in presentation mode
				Selected = null;
			}

			RefreshIfVisible();
		}

		private void UpdateVisibility()
		{
			// Ensure we leave presentation mode when we leave a call
			if (m_SubscribedPresentationComponent != null)
				StopPresenting();

			bool isInCall = ActiveConferenceControl != null && ActiveConferenceControl.GetActiveConferences().Any();
			if (!isInCall)
				ShowView(false);
		}

		private void StartPresenting(ISource source)
		{
			if (Room == null || source == null)
				return;

			IDevice sourceDevice = Room.Core.Originators.GetChild(source.Device) as IDevice;
			if (sourceDevice is VibeBoard)
				sourceDevice.Controls.GetControl<VibeBoardAppControl>().LaunchApp(eVibeApp.Whiteboard);
			else
				Navigation.NavigateTo<IBackgroundPresenter>().Refresh();

			Room.Routing.RouteToVtcPresentation(source, m_SubscribedPresentationComponent);
		}

		private void StopPresenting()
		{
			if (Room == null)
				return;

			Room.Routing.UnrouteAllFromVtcPresentation(m_SubscribedPresentationComponent);
			Navigation.NavigateTo<IBackgroundPresenter>().Refresh();
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

			if (room.Core.TryGetRoutingGraph(out m_SubscribedRoutingGraph) && m_SubscribedRoutingGraph != null)
				m_SubscribedRoutingGraph.RoutingCache.OnEndpointRouteChanged += RoutingCacheOnEndpointRouteChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedRoutingGraph != null)
				m_SubscribedRoutingGraph.RoutingCache.OnEndpointRouteChanged -= RoutingCacheOnEndpointRouteChanged;

			m_SubscribedRoutingGraph = null;
		}

		private void RoutingCacheOnEndpointRouteChanged(object sender, EndpointRouteChangedEventArgs eventArgs)
		{
			if (eventArgs.Type.HasFlag(eConnectionType.Video))
				UpdatePresentationRoutedSources();
		}

		#endregion

		#region Conference Control Callbacks

		protected override void Subscribe(IConferenceDeviceControl control)
		{
			if (control != null)
			{
				control.OnConferenceAdded += VideoDialerOnConferenceAdded;
				control.OnConferenceRemoved += VideoDialerOnConferenceRemoved;
			}

			var device = control == null ? null : control.Parent;
			m_SubscribedPresentationComponent = device == null ? null : device.Controls.GetControl<IPresentationControl>();

			if (m_SubscribedPresentationComponent == null)
				return;

			m_SubscribedPresentationComponent.OnPresentationActiveInputChanged += SubscribedPresentationControlOnPresentationActiveInputChanged;

			UpdateSources();
		}

		protected override void Unsubscribe(IConferenceDeviceControl control)
		{
			if (control != null)
			{
				control.OnConferenceAdded -= VideoDialerOnConferenceAdded;
				control.OnConferenceRemoved -= VideoDialerOnConferenceRemoved;
			}

			if (m_SubscribedPresentationComponent != null)
				m_SubscribedPresentationComponent.OnPresentationActiveInputChanged -= SubscribedPresentationControlOnPresentationActiveInputChanged;

			m_SubscribedPresentationComponent = null;

			UpdateSources();
		}

		private void SubscribedPresentationControlOnPresentationActiveInputChanged(object sender, EventArgs eventArgs)
		{
			UpdatePresentationRoutedSources();
		}

		private void VideoDialerOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data);

			UpdateVisibility();
		}

		private void VideoDialerOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data);

			UpdateVisibility();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs e)
		{
			UpdateVisibility();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IShareConferenceView view)
		{
			base.Subscribe(view);

			view.OnSourceButtonPressed += ViewOnSourceButtonPressed;
			view.OnShareButtonPressed += ViewOnShareButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IShareConferenceView view)
		{
			base.Unsubscribe(view);

			view.OnSourceButtonPressed -= ViewOnSourceButtonPressed;
			view.OnShareButtonPressed -= ViewOnShareButtonPressed;
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// Clear the selection state when visibility changes
			Selected = null;
		}

		private void ViewOnSourceButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			ISource source;
			if (!m_Sources.TryElementAt(eventArgs.Data, out source))
				return;

			if (IsInPresentation())
			{
				StartPresenting(source);
				return;
			}

			Selected = source == Selected ? null : source;
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

		#endregion
	}
}