using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.Extensions;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	[PresenterBinding(typeof(IWtcSharePresenter))]
	public sealed class WtcSharePresenter : AbstractWtcPresenter<IWtcShareView>, IWtcSharePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly IcdHashSet<ISource> m_RoutedSources;
		private readonly List<ISource> m_Sources;

		private ISource m_Selected;

		[CanBeNull]
		private IPresentationControl m_SubscribedPresentationComponent;

		[CanBeNull]
		private IRoutingGraph m_SubscribedRoutingGraph;

		#region Properties

		/// <summary>
		/// Gets/sets the selected source.
		/// </summary>
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
		/// Returns true if we are currently presenting a source.
		/// </summary>
		/// <value></value>
		private bool IsPresenting
		{
			get
			{
				return m_SubscribedPresentationComponent != null &&
				       m_SubscribedPresentationComponent.IsNearSidePresenting();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcSharePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_RoutedSources = new IcdHashSet<ISource>();
			m_Sources = new List<ISource>();
		}

		#region Methods

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcShareView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool inPresentation = IsPresenting;

				for (ushort index = 0; index < m_Sources.Count; index++)
				{
					ISource source = m_Sources[index];
					ConnectProSource connectProSource = source as ConnectProSource;

					IRoom room = Room == null ? null : Room.Routing.Sources.GetRoomForSource(source);
					bool combine = room != null && room.CombineState;

					string icon =
						connectProSource == null
						? null
						: Icons.GetSourceIcon(connectProSource.Icon, eSourceColor.White);

					bool select = inPresentation ? m_RoutedSources.Contains(source) : source == m_Selected;
					bool enable = source.EnableWhenOffline || source.GetDevices().All(d => d.IsOnline); 

					view.SetButtonEnabled(index, enable);
					view.SetButtonSelected(index, select);
					view.SetButtonIcon(index, icon);
					view.SetButtonLabel(index, source == null ? null : source.GetName(combine));
				}

				bool enabled = inPresentation || m_Selected != null;

				view.SetButtonCount((ushort)m_Sources.Count);
				view.SetShareButtonEnabled(enabled);
				view.SetShareButtonSelected(inPresentation);
				view.SetSwipeLabelsVisible(m_Sources.Count > 4);
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

		#endregion

		#region Private Methods

		/// <summary>
		/// Selects the given source.
		/// If we're already in a presentation we present this source instead.
		/// If not in a presentation and the source is already selected, unselects the source.
		/// </summary>
		/// <param name="source"></param>
		private void ToggleSelection([NotNull] ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (IsPresenting)
			{
				StartPresenting(source);
				return;
			}

			// Toggle the selection
			if (source == Selected)
				ClearSelection();
			else
				Selected = source;
		}

		/// <summary>
		/// Resets the source selection to the default state.
		/// If there is only 1 source we select it by default.
		/// </summary>
		private void ClearSelection()
		{
			Selected = m_Sources.Count == 1 ? m_Sources[0] : null;
		}

		/// <summary>
		/// Updates the list of sources available for presentation.
		/// </summary>
		private void UpdateSources()
		{
			m_Sources.SelectMany(s => s.GetDevices()).ForEach(Unsubscribe);

			m_Sources.Clear();
			m_Sources.AddRange(GetSources());

			m_Sources.SelectMany(s => s.GetDevices()).ForEach(Subscribe);

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
		/// Updates the sources that are currently routed to the presentation input.
		/// </summary>
		private void UpdatePresentationRoutedSources()
		{
			IcdHashSet<ISource> routed =
				Room == null || m_SubscribedPresentationComponent == null
					? new IcdHashSet<ISource>()
					: Room.Routing
					      .Sources
					      .GetVtcPresentationSources(m_SubscribedPresentationComponent)
					      .ToIcdHashSet();

			if (routed.SetEquals(m_RoutedSources))
				return;

			m_RoutedSources.Clear();
			m_RoutedSources.AddRange(routed);

			// Update the selection to reflect the current presenting source
			if (!IsPresenting || m_RoutedSources.Count == 0)
				return;

			if (Selected != null && m_RoutedSources.Contains(Selected))
				return;

			Selected = m_RoutedSources.First();
		}

		/// <summary>
		/// Hides the view if we are no longer in a call.
		/// </summary>
		private void UpdateVisibility()
		{
			// Ensure we leave presentation mode when we leave a call
			if (m_SubscribedPresentationComponent != null)
				StopPresenting();

			bool isInCall = ActiveConferenceControl != null && ActiveConferenceControl.GetActiveConference() != null;
			if (!isInCall)
				ShowView(false);
		}

		/// <summary>
		/// Routes the given source and starts the presentation.
		/// </summary>
		/// <param name="source"></param>
		private void StartPresenting([NotNull] ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (Room == null || m_SubscribedPresentationComponent == null)
				return;

			Room.Routing.RouteToVtcPresentation(source, m_SubscribedPresentationComponent);
		}

		/// <summary>
		/// Unroutes the current source and stops the active presentation.
		/// </summary>
		private void StopPresenting()
		{
			if (Room == null || m_SubscribedPresentationComponent == null)
				return;

			Room.Routing.UnrouteAllFromVtcPresentation(m_SubscribedPresentationComponent);
		}

		#endregion

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

		/// <summary>
		/// Called when any endpoint to endpoint route changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoutingCacheOnEndpointRouteChanged(object sender, EndpointRouteChangedEventArgs eventArgs)
		{
			if (eventArgs.Type.HasFlag(eConnectionType.Video))
				UpdatePresentationRoutedSources();
		}

		#endregion

		#region Conference Control Callbacks

		/// <summary>
		/// Subscribe to the web conference control.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			if (control != null)
			{
				control.OnConferenceAdded += VideoDialerOnConferenceAdded;
				control.OnConferenceRemoved += VideoDialerOnConferenceRemoved;
			}

			IDevice device = control == null ? null : control.Parent;
			m_SubscribedPresentationComponent = device == null ? null : device.Controls.GetControl<IPresentationControl>();

			if (m_SubscribedPresentationComponent != null)
			{
				m_SubscribedPresentationComponent.OnPresentationActiveInputChanged += SubscribedPresentationControlOnPresentationActiveInputChanged;
				m_SubscribedPresentationComponent.OnPresentationActiveChanged += SubscribedPresentationComponentOnPresentationActiveChanged;
			}

			UpdateSources();
		}

		/// <summary>
		/// Unsubscribe from the web conference control.
		/// </summary>
		/// <param name="control"></param>
		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			if (control != null)
			{
				control.OnConferenceAdded -= VideoDialerOnConferenceAdded;
				control.OnConferenceRemoved -= VideoDialerOnConferenceRemoved;
			}

			if (m_SubscribedPresentationComponent != null)
			{
				m_SubscribedPresentationComponent.OnPresentationActiveInputChanged -= SubscribedPresentationControlOnPresentationActiveInputChanged;
				m_SubscribedPresentationComponent.OnPresentationActiveChanged -= SubscribedPresentationComponentOnPresentationActiveChanged;
			}
			m_SubscribedPresentationComponent = null;

			UpdateSources();
		}

		/// <summary>
		/// Called when presentation starts/stops.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="presentationActiveApiEventArgs"></param>
		private void SubscribedPresentationComponentOnPresentationActiveChanged(object sender, PresentationActiveApiEventArgs presentationActiveApiEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the active presentation input changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void SubscribedPresentationControlOnPresentationActiveInputChanged(object sender, EventArgs eventArgs)
		{
			UpdatePresentationRoutedSources();

			RefreshIfVisible();
		}

		/// <summary>
		/// Called when a video conference starts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VideoDialerOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data as IWebConference);

			UpdateVisibility();
		}

		/// <summary>
		/// Called when a video conference ends.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VideoDialerOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data as IWebConference);

			UpdateVisibility();
		}

		#endregion

		#region Conference Callbacks

		/// <summary>
		/// Subscribe to the web conference events.
		/// </summary>
		/// <param name="conference"></param>
		private void Subscribe(IWebConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the web conference events.
		/// </summary>
		/// <param name="conference"></param>
		private void Unsubscribe(IWebConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		/// <summary>
		/// Called when a web conference status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		protected override void Subscribe(IWtcShareView view)
		{
			base.Subscribe(view);

			view.OnSourceButtonPressed += ViewOnSourceButtonPressed;
			view.OnShareButtonPressed += ViewOnShareButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcShareView view)
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

			ClearSelection();
		}

		/// <summary>
		/// Called when the user presses a source button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSourceButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			ISource source;
			if (m_Sources.TryElementAt(eventArgs.Data, out source))
				ToggleSelection(source);
		}

		/// <summary>
		/// Called when the user presses the share button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnShareButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			if (IsPresenting)
				StopPresenting();
			else
				StartPresenting(m_Selected);
		}

		#endregion

		#region Device Callbacks

		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="device"></param>
		private void Subscribe(IDeviceBase device)
		{
			device.OnIsOnlineStateChanged += DeviceOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="device"></param>
		private void Unsubscribe(IDeviceBase device)
		{
			device.OnIsOnlineStateChanged += DeviceOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when the device online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void DeviceOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
