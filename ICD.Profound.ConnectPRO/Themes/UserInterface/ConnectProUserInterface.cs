using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Options;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class ConnectProUserInterface : IUserInterface
	{
		private const long SOURCE_SELECTION_TIMEOUT = 8 * 1000;

		private readonly IPanelDevice m_Panel;
		private readonly IConnectProNavigationController m_NavigationController;
		private readonly SafeTimer m_SourceSelectionTimeout;

		private readonly SafeCriticalSection m_RoutingSection;

		private IConnectProRoom m_Room;
		private DefaultVisibilityNode m_RootVisibility;
		private ISource m_SelectedSource;
		private bool m_CombinedAdvancedMode;

		#region Properties

		public IPanelDevice Panel { get { return m_Panel; } }

		public IConnectProRoom Room { get { return m_Room; } }

		object IUserInterface.Target { get { return Panel; } }

		public bool CombinedAdvancedMode
		{
			get { return m_CombinedAdvancedMode; } 
			set 
			{
				if (m_CombinedAdvancedMode == value)
					return;

				m_CombinedAdvancedMode = value;
				SetSelectedSource(null);
				UpdateMeetingPresentersVisibility();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProUserInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			m_RoutingSection = new SafeCriticalSection();

			m_Panel = panel;
			UpdatePanelOnlineJoin();

			m_SourceSelectionTimeout = SafeTimer.Stopped(() => SetSelectedSource(null));

			IUiViewFactory viewFactory = new ConnectProUiViewFactory(panel, theme);
			m_NavigationController = new ConnectProNavigationController(viewFactory, theme);

			BuildVisibilityTree();
			SubscribePresenters();
		}

		/// <summary>
		/// Updates the "offline" visual state of the panel
		/// </summary>
		private void UpdatePanelOnlineJoin()
		{
			m_Panel.SendInputDigital(CommonJoins.DIGITAL_OFFLINE_JOIN, m_Room == null);
		}

		/// <summary>
		/// Builds the rules for view visibility, e.g. prevent certain items from being visible at the same time.
		/// </summary>
		private void BuildVisibilityTree()
		{
			// Only allow one of the start/end buttons to be visible at any given time
			m_RootVisibility = new DefaultVisibilityNode(m_NavigationController.LazyLoadPresenter<IStartMeetingPresenter>());

			IVisibilityNode displaysVisibility = new SingleVisibilityNode();
			displaysVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>());
			displaysVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>());
			displaysVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>());

			m_RootVisibility.AddNode(displaysVisibility);

			// Video Conference node
			IVisibilityNode videoConferencingVisibility = new SingleVisibilityNode();
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcContactsNormalPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcContactsPolycomPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcSharePresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcDtmfPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcActiveCallsPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcKeyboardPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcKeypadPresenter>());
			
			// Web Conference node
			IVisibilityNode webConferencingVisibility = new SingleVisibilityNode();
			webConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcCallOutPresenter>());
			webConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcSharePresenter>());
			webConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcContactListPresenter>());
			webConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcRecordingPresenter>());
			webConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcActiveMeetingPresenter>());
			webConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcStartMeetingPresenter>());

			// Audio Conference node
			IVisibilityNode audioConferencingVisibility = new SingleVisibilityNode();
			audioConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IAtcBasePresenter>());

			// Meeting node
			IVisibilityNode meetingVisibility = new VisibilityNode();

			meetingVisibility.AddNode(videoConferencingVisibility);
			meetingVisibility.AddNode(audioConferencingVisibility);
			meetingVisibility.AddNode(webConferencingVisibility);

			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IEndMeetingPresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOptionPrivacyMutePresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOptionVolumePresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOptionCameraPresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcBasePresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcBasePresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ICableTvPresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWebConferencingAlertPresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWebConferencingStepPresenter>());

			m_RootVisibility.AddNode(meetingVisibility);

			// Camera visibility
			IVisibilityNode cameraVisibility = new SingleVisibilityNode();
			cameraVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ICameraControlPresenter>());
			cameraVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcLeftMenuPresenter>());
			cameraVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcButtonListPresenter>());
			cameraVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcContactsNormalPresenter>());
			cameraVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcContactsPolycomPresenter>());

			// Settings node
			IVisibilityNode settingsVisibility = new SingleVisibilityNode();
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsPasscodePresenter>());
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsSystemPowerPresenter>());
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsDirectoryPresenter>());
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsRoomCombinePresenter>());

			m_RootVisibility.AddNode(settingsVisibility);

			// These presenters are initially visible.
			m_NavigationController.NavigateTo<IHeaderPresenter>();
			m_NavigationController.NavigateTo<IHardButtonsPresenter>();

			// These presenters control their own visibility.
			m_NavigationController.LazyLoadPresenter<IEndMeetingPresenter>();
			m_NavigationController.LazyLoadPresenter<IStartMeetingPresenter>();
			m_NavigationController.LazyLoadPresenter<IOptionPrivacyMutePresenter>();
			m_NavigationController.LazyLoadPresenter<IOptionVolumePresenter>();
			m_NavigationController.LazyLoadPresenter<IOptionCameraPresenter>();
			m_NavigationController.LazyLoadPresenter<IVtcCallListTogglePresenter>();
			m_NavigationController.LazyLoadPresenter<IVtcIncomingCallPresenter>();
			m_NavigationController.LazyLoadPresenter<IAtcIncomingCallPresenter>();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			UnsubscribePresenters();

			SetRoom(null);

			m_SourceSelectionTimeout.Dispose();
			m_NavigationController.Dispose();
		}

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			if (room == m_Room)
				return;

			Unsubscribe(m_Room);

			ServiceProvider.GetService<ILoggerService>()
			               .AddEntry(eSeverity.Informational, "{0} setting room to {1}", this, room);

			m_Room = room;

			m_NavigationController.SetRoom(room);

			Subscribe(m_Room);

			UpdateMeetingPresentersVisibility();

			UpdateRouting();
			UpdateRoutedSources();

			UpdatePanelOnlineJoin();
		}

		/// <summary>
		/// Gets the string representation for this instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			ReprBuilder builder = new ReprBuilder(this);
			builder.AppendProperty("Panel", m_Panel);
			return builder.ToString();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called to update the selection state of the given source.
		/// </summary>
		/// <param name="source"></param>
		private void HandleSelectedSource(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (m_Room == null)
				return;

			// If the source is already active then clear the active state
			m_RoutingSection.Enter();
			try
			{
				if (source == m_SelectedSource)
				{
					SetSelectedSource(null);
					return;
				}
			}
			finally
			{
				m_RoutingSection.Leave();
			}

			if (m_Room.IsCombineRoom())
			{
				if (CombinedAdvancedMode)
					HandleSelectedSourceCombinedAdvancedMode(source);
				else
					HandleSelectedSourceCombinedSimpleMode(source);
			}
			else
			{
				if (m_Room.Routing.Destinations.IsDualDisplayRoom)
					HandleSelectedSourceDualDisplay(source);
				else
					HandleSelectedSourceSingleDisplay(source);
			}
		}

		/// <summary>
		/// In dual display mode we allow the user to select which display to route to.
		/// </summary>
		/// <param name="source"></param>
		private void HandleSelectedSourceDualDisplay(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDeviceBase device = m_Room.Core.Originators.GetChild<IDeviceBase>(source.Device);
			IConferenceDeviceControl dialer = device.Controls.GetControl<IConferenceDeviceControl>();

			// Edge case - route the codec to both displays and open the context menu
			if (dialer != null && dialer.Supports.HasFlag(eCallType.Video))
			{
				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source, false);

				IRouteSourceControl sourceControl = m_Room.Core.GetControl<IRouteSourceControl>(source.Device, source.Control);
				m_Room.Routing.RouteVtc(sourceControl);
			}
			// Edge case - open the audio conferencing context menu
			else if (dialer != null && dialer.Supports.HasFlag(eCallType.Audio))
			{
				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source, false);

				IRouteSourceControl sourceControl = m_Room.Core.GetControl<IRouteSourceControl>(source.Device, source.Control);
				m_Room.Routing.RouteAtc(sourceControl);
			}
			// Typical case - continue routing
			else
			{
				SetSelectedSource(source);
				return;
			}

			m_SourceSelectionTimeout.Reset(SOURCE_SELECTION_TIMEOUT);
		}

		/// <summary>
		/// In single display mode we route the source immediately.
		/// </summary>
		/// <param name="source"></param>
		private void HandleSelectedSourceSingleDisplay(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			// Is the source already routed?     
			if (m_Room.Routing.State.GetIsRoutedCached(source, eConnectionType.Video))
			{
				ShowSourceContextualMenu(source, false);
			}
			else
			{
				m_Room.Routing.State.SetProcessingSource(source);

				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source, false);

				m_Room.Routing.RouteSingleDisplay(source);
			}

			SetSelectedSource(null);
		}

		/// <summary>
		/// In combined simple mode we route the source immediately.
		/// </summary>
		/// <param name="source"></param>
		private void HandleSelectedSourceCombinedSimpleMode(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			// Is the source already routed?     
			if (m_Room.Routing.State.GetIsRoutedCached(source, eConnectionType.Video))
			{
				ShowSourceContextualMenu(source, false);
			}
			else
			{
				m_Room.Routing.State.SetProcessingSource(source);

				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source, false);

				m_Room.Routing.RouteAllDisplays(source);
			}

			SetSelectedSource(null);
		}

		private void HandleSelectedSourceCombinedAdvancedMode(ISource source)
		{
			// Todo - Copied from dual display, make it unique to combined advanced mode?
			if (source == null)
				throw new ArgumentNullException("source");

			IDeviceBase device = m_Room.Core.Originators.GetChild<IDeviceBase>(source.Device);
			IConferenceDeviceControl dialer = device.Controls.GetControl<IConferenceDeviceControl>();

			// Edge case - route the codec to both displays and open the context menu
			if (dialer != null && dialer.Supports.HasFlag(eCallType.Video))
			{
				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source, false);

				IRouteSourceControl sourceControl = m_Room.Core.GetControl<IRouteSourceControl>(source.Device, source.Control);
				m_Room.Routing.RouteVtc(sourceControl);
			}
			// Edge case - open the audio conferencing context menu
			else if (dialer != null && dialer.Supports.HasFlag(eCallType.Audio))
			{
				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source, false);

				IRouteSourceControl sourceControl = m_Room.Core.GetControl<IRouteSourceControl>(source.Device, source.Control);
				m_Room.Routing.RouteAtc(sourceControl);
			}
			// Typical case - continue routing
			else
			{
				SetSelectedSource(source);
				return;
			}

			m_SourceSelectionTimeout.Reset(SOURCE_SELECTION_TIMEOUT);
		}

		/// <summary>
		/// Called to update the selection state of the given destination.
		/// </summary>
		/// <param name="routedSource"></param>
		/// <param name="destination"></param>
		private void HandleSelectedDisplay(ISource routedSource, IDestination destination)
		{
			if (m_Room == null)
				return;

			if (destination == null)
				return;

			// Store in local variable because route feedback will change the field
			ISource activeSource = m_RoutingSection.Execute(() => m_SelectedSource);

			// If no source is selected for routing then we open the contextual menu for the current routed source
			if (activeSource == null || activeSource == routedSource)
			{
				ShowSourceContextualMenu(routedSource, false);
			}
			// If a source is currently selected then we route that source to the selected display
			else if (activeSource != routedSource)
			{
				m_RoutingSection.Enter();

				try
				{
					m_Room.Routing.State.SetProcessingSource(destination, activeSource);
				}
				finally
				{
					m_RoutingSection.Leave();
				}

				m_Room.Routing.RouteDualDisplay(activeSource, destination);
				
				routedSource = activeSource;

				if (ShowSourceContextualMenu(routedSource, true))
					SetSelectedSource(null);
			}
		}

		private bool ShowSourceContextualMenu(ISource source, bool vtcOnly)
		{
			if (m_Room == null)
				return false;

			if (source == null)
				return false;

			eControlOverride controlOverride = ConnectProRoutingSources.GetControlOverride(source);
			IDeviceControl control = ConnectProRoutingSources.GetDeviceControl(source, controlOverride);

			if (control is IConferenceDeviceControl)
			{
				ITraditionalConferenceDeviceControl dialer = control as ITraditionalConferenceDeviceControl;

				if (dialer != null)
				{
					if (dialer.Supports.HasFlag(eCallType.Video))
					{
						var vtcPresenter = m_NavigationController.LazyLoadPresenter<IVtcBasePresenter>();
						vtcPresenter.ActiveConferenceControl = dialer;
						vtcPresenter.ShowView(true);
					}
					else
					{
						var atcPresenter = m_NavigationController.LazyLoadPresenter<IAtcBasePresenter>();
						atcPresenter.ActiveConferenceControl = dialer;
						atcPresenter.ShowView(true);
					}
				}

				IWebConferenceDeviceControl webControl = control as IWebConferenceDeviceControl;
				if (webControl != null) {
					var wtcPresenter = m_NavigationController.LazyLoadPresenter<IWtcBasePresenter>();
					wtcPresenter.ActiveConferenceControl = webControl;
					wtcPresenter.ShowView(true);
				}

				SetSelectedSource(null);
				return true;
			}

			if (vtcOnly)
				return false;

			if (control is ITvTunerControl)
			{
				m_NavigationController.NavigateTo<ICableTvPresenter>().Control = control as ITvTunerControl;
				SetSelectedSource(null);
				return true;
			}

			switch (controlOverride)
			{
				case eControlOverride.WebConference:
					m_NavigationController.NavigateTo<IWebConferencingAlertPresenter>();
					SetSelectedSource(null);
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Sets the source that is currently selected for routing to the displays.
		/// </summary>
		/// <param name="source"></param>
		private void SetSelectedSource(ISource source)
		{
			m_RoutingSection.Enter();

			try
			{
				if (source == m_SelectedSource)
					return;

				m_SelectedSource = source;

				m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().SelectedSource = m_SelectedSource;
				m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>().SelectedSource = m_SelectedSource;
				m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>().SelectedSource = m_SelectedSource;
				m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>().SelectedSource = m_SelectedSource;

				m_SourceSelectionTimeout.Reset(SOURCE_SELECTION_TIMEOUT);
			}
			finally
			{
				m_RoutingSection.Leave();
			}
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Subscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;

			room.Routing.State.OnDisplaySourceChanged += RoutingOnDisplaySourceChanged;
			room.Routing.State.OnAudioSourceChanged += RoutingOnAudioSourceChanged;
			room.Routing.State.OnSourceRoutedChanged += RoutingOnSourceRoutedChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;

			room.Routing.State.OnDisplaySourceChanged -= RoutingOnDisplaySourceChanged;
			room.Routing.State.OnAudioSourceChanged -= RoutingOnAudioSourceChanged;
			room.Routing.State.OnSourceRoutedChanged -= RoutingOnSourceRoutedChanged;
		}

		/// <summary>
		/// Called when the room enters/exits a meeting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			SetSelectedSource(null);

			if (m_Room != null)
				m_Room.Routing.State.ClearProcessingSources();

			UpdateMeetingPresentersVisibility();
		}

		/// <summary>
		/// Sets the visibility of the subpages based on the meeting state.
		/// </summary>
		private void UpdateMeetingPresentersVisibility()
		{
			bool isInMeeting = m_Room != null && m_Room.IsInMeeting;

			// Set the visibility of the meeting buttons
			m_NavigationController.LazyLoadPresenter<IStartMeetingPresenter>().ShowView(!isInMeeting);
			m_NavigationController.LazyLoadPresenter<IEndMeetingPresenter>().ShowView(isInMeeting);

			bool combinedRoom = m_Room != null && m_Room.IsCombineRoom();
			bool dualDisplays = m_Room != null && !combinedRoom && m_Room.Routing.Destinations.IsDualDisplayRoom;
			bool combineAdvanced = m_Room != null && combinedRoom && CombinedAdvancedMode;
			bool combineSimple = m_Room != null && combinedRoom && !CombinedAdvancedMode;

			m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().ShowView(true);
			m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>().ShowView(dualDisplays);
			m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>().ShowView(combineAdvanced);
			m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>().ShowView(combineSimple);
		}

		private void RoutingOnAudioSourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateRouting();
		}

		private void RoutingOnDisplaySourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateRouting();
		}

		private void RoutingOnSourceRoutedChanged(object sender, EventArgs eventArgs)
		{
			UpdateRoutedSources();
		}

		private void UpdateRoutedSources()
		{
			Dictionary<ISource, eSourceState> routedSources =
				m_Room == null
					? new Dictionary<ISource, eSourceState>()
					: m_Room.Routing.State.GetSourceRoutedStates().ToDictionary();

			m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().SetRoutedSources(routedSources);
		}

		private void UpdateRouting()
		{
			IcdHashSet<ISource> activeAudio =
				m_Room == null
					? new IcdHashSet<ISource>()
					: m_Room.Routing.State.GetCachedActiveAudioSources().ToIcdHashSet();

			Dictionary<IDestination, IcdHashSet<ISource>> activeVideo =
				m_Room == null
					? new Dictionary<IDestination, IcdHashSet<ISource>>()
					: m_Room.Routing.State.GetCachedActiveVideoSources().ToDictionary();

			m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>()
								  .SetRouting(activeVideo, activeAudio);
			m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>()
								  .SetRouting(activeVideo, activeAudio);
			m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>()
								  .SetRouting(activeVideo, activeAudio);
			m_NavigationController.LazyLoadPresenter<IMenuRouteSummaryPresenter>()
								  .SetRouting(activeVideo);

			// If the active source is routed to all destinations we clear the active source
			if (m_SelectedSource != null && activeVideo.All(kvp => kvp.Value.Contains(m_SelectedSource)))
				SetSelectedSource(null);
		}

		#endregion

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		private void SubscribePresenters()
		{
			Subscribe(m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>());
			Subscribe(m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>());

			Subscribe(m_NavigationController.LazyLoadPresenter<IVtcIncomingCallPresenter>());
			Subscribe(m_NavigationController.LazyLoadPresenter<IAtcIncomingCallPresenter>());

			Subscribe(m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>());
			Subscribe(m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>());
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		private void UnsubscribePresenters()
		{
			Unsubscribe(m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>());
			Unsubscribe(m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>());

			Unsubscribe(m_NavigationController.LazyLoadPresenter<IVtcIncomingCallPresenter>());
			Unsubscribe(m_NavigationController.LazyLoadPresenter<IAtcIncomingCallPresenter>());

			Unsubscribe(m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>());
			Unsubscribe(m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>());
		}

		#region Source Select Callbacks

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(ISourceSelectPresenter presenter)
		{
			presenter.OnSourcePressed += SourceSelectPresenterOnSourcePressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(ISourceSelectPresenter presenter)
		{
			presenter.OnSourcePressed -= SourceSelectPresenterOnSourcePressed;
		}

		/// <summary>
		/// Called when the user presses a source in the presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="source"></param>
		private void SourceSelectPresenterOnSourcePressed(object sender, ISource source)
		{
			if (source != null)
				HandleSelectedSource(source);
		}

		#endregion

		#region Menu Displays Callbacks

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IMenuDisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed += DisplaysPresenterOnDestinationPressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IMenuDisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed -= DisplaysPresenterOnDestinationPressed;
		}

		/// <summary>
		/// Called when the user presses a destination in the presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedSource"></param>
		/// <param name="destination"></param>
		private void DisplaysPresenterOnDestinationPressed(object sender, ISource routedSource, IDestination destination)
		{
			HandleSelectedDisplay(routedSource, destination);
		}

		#endregion

		#region Incoming Video Call Presenter Callbacks

		private void Subscribe(IVtcIncomingCallPresenter presenter)
		{
			presenter.OnCallAnswered += VtcIncomingCallPresenterOnCallAnswered;
		}

		private void Unsubscribe(IVtcIncomingCallPresenter presenter)
		{
			presenter.OnCallAnswered -= VtcIncomingCallPresenterOnCallAnswered;
		}

		private void VtcIncomingCallPresenterOnCallAnswered(object sender, GenericEventArgs<IConferenceDeviceControl> e)
		{
			if (m_Room == null)
				return;

			IConferenceDeviceControl videoDialer = e.Data;
			IDeviceBase device = videoDialer == null ? null : videoDialer.Parent;

			if (device == null)
				return;

			ISource source = m_Room.Originators.GetInstancesRecursive<ISource>(s => s.Device == device.Id).FirstOrDefault();
			if (source == null)
				return;

			HandleSelectedSource(source);
		}

		#endregion

		#region Incoming Audio Call Presenter Callbacks

		private void Subscribe(IAtcIncomingCallPresenter presenter)
		{
			presenter.OnCallAnswered += AtcIncomingCallPresenterOnCallAnswered;
		}

		private void Unsubscribe(IAtcIncomingCallPresenter presenter)
		{
			presenter.OnCallAnswered -= AtcIncomingCallPresenterOnCallAnswered;
		}

		private void AtcIncomingCallPresenterOnCallAnswered(object sender, GenericEventArgs<IConferenceDeviceControl> e)
		{
			if (m_Room == null)
				return;

			IConferenceDeviceControl audioDialer = e.Data;
			IDeviceBase device = audioDialer == null ? null : audioDialer.Parent;

			if (device == null)
				return;

			ISource source = m_Room.Originators.GetInstancesRecursive<ISource>(s => s.Device == device.Id).FirstOrDefault();
			if (source == null)
				return;

			HandleSelectedSource(source);
		}

		#endregion

		#region Combined Display Presenters Callbacks

		private void Subscribe(IMenuCombinedAdvancedModePresenter presenter)
		{
			presenter.OnSimpleModePressed += PresenterOnSimpleModePressed;
			presenter.OnDestinationPressed += DisplaysPresenterOnDestinationPressed;
		}

		private void Unsubscribe(IMenuCombinedAdvancedModePresenter presenter)
		{
			presenter.OnSimpleModePressed -= PresenterOnSimpleModePressed;
			presenter.OnDestinationPressed -= DisplaysPresenterOnDestinationPressed;
		}
		
		private void Subscribe(IMenuCombinedSimpleModePresenter presenter)
		{
			presenter.OnAdvancedModePressed += PresenterOnAdvancedModePressed;
			presenter.OnDestinationPressed += DisplaysPresenterOnDestinationPressed;
		}

		private void Unsubscribe(IMenuCombinedSimpleModePresenter presenter)
		{
			presenter.OnAdvancedModePressed -= PresenterOnAdvancedModePressed;
			presenter.OnDestinationPressed -= DisplaysPresenterOnDestinationPressed;
		}
		
		private void PresenterOnSimpleModePressed(object sender, EventArgs e)
		{
			CombinedAdvancedMode = false;
		}

		private void PresenterOnAdvancedModePressed(object sender, EventArgs e)
		{
			CombinedAdvancedMode = true;
		}

		#endregion
	}
}
