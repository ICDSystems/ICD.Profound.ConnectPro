using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
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
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Rooms.Combine;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class ConnectProUserInterface : AbstractUserInterface
	{
		private const long SOURCE_SELECTION_TIMEOUT = 8 * 1000;

		private static readonly Dictionary<eControlOverride, Type> s_OverrideToPresenterType =
			new Dictionary<eControlOverride, Type>
			{
				{eControlOverride.WebConference, typeof(IWebConferencingAlertPresenter)},
				{eControlOverride.CableTv, typeof(ICableTvPresenter)},
				{eControlOverride.Vtc, typeof(IVtcBasePresenter)},
				{eControlOverride.Atc, typeof(IAtcBasePresenter)}
			};

		/// <summary>
		/// Maps control types to potential context menu presenter types.
		/// Checks each presenter type for compatibility and falls back to the next.
		/// </summary>
		private static readonly Dictionary<Type, List<Type>> s_ControlToPresenterType =
			new Dictionary<Type, List<Type>>
			{
				{typeof(ITraditionalConferenceDeviceControl), new List<Type> {typeof(IVtcBasePresenter), typeof(IAtcBasePresenter)}},
				{typeof(IWebConferenceDeviceControl), new List<Type> {typeof(IWtcBasePresenter)}},
				{typeof(ITvTunerControl), new List<Type> {typeof(ICableTvPresenter)}}
			};

// ReSharper disable NotAccessedField.Local
		private readonly ConnectProVisibilityTree m_VisibilityTree;
// ReSharper restore NotAccessedField.Local

		private readonly IPanelDevice m_Panel;
		private readonly ConnectProTheme m_Theme;
		private readonly IConnectProNavigationController m_NavigationController;
		private readonly SafeTimer m_SourceSelectionTimeout;

		private readonly SafeCriticalSection m_RoutingSection;

		private IConnectProRoom m_Room;
		private ISource m_SelectedSource;
		private bool m_UserInterfaceReady;

		#region Properties

		public IPanelDevice Panel { get { return m_Panel; } }

		public override IRoom Room { get { return ConnectProRoom; } }

		public IConnectProRoom ConnectProRoom { get { return m_Room; } }

		public override object Target { get { return m_Panel; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProUserInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			if (panel == null)
				throw new ArgumentNullException("panel");

			if (theme == null)
				throw new ArgumentNullException("theme");

			m_Panel = panel;
			m_Theme = theme;

			m_RoutingSection = new SafeCriticalSection();

			UpdatePanelOfflineJoin();

			m_SourceSelectionTimeout = SafeTimer.Stopped(() => SetSelectedSource(null));

			IUiViewFactory viewFactory = new ConnectProUiViewFactory(panel, theme);
			m_NavigationController = new ConnectProNavigationController(viewFactory, theme);
			m_VisibilityTree = new ConnectProVisibilityTree(m_NavigationController);

			SubscribePresenters();
			Subscribe(m_Theme);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			UnsubscribePresenters();
			Unsubscribe(m_Theme);

			SetRoom(null);

			m_SourceSelectionTimeout.Dispose();
			m_NavigationController.Dispose();
		}

		/// <summary>
		/// Updates the "offline" visual state of the panel
		/// </summary>
		private void UpdatePanelOfflineJoin()
		{
			m_Panel.SendInputDigital(CommonJoins.DIGITAL_OFFLINE_JOIN, m_Room == null || !m_UserInterfaceReady);
		}

		#region Methods

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IRoom room)
		{
			SetRoom(room as IConnectProRoom);
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

			UpdatePanelOfflineJoin();
		}

		/// <summary>
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
			m_UserInterfaceReady = true;
			UpdatePanelOfflineJoin();
		}

		#endregion

		#region Source/Destination Selection

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

			ConnectProCombineRoom combineRoom = m_Room as ConnectProCombineRoom;
			if (combineRoom == null)
			{
				if (m_Room.Routing.Destinations.IsMultiDisplayRoom)
					HandleSelectedSourceMultiDisplay(source);
				else
					HandleSelectedSourceSingleDisplay(source);
			}
			else
			{
				switch (combineRoom.CombinedAdvancedMode)
				{
					case eCombineAdvancedMode.Simple:
						HandleSelectedSourceCombinedSimpleMode(source);
						break;
					case eCombineAdvancedMode.Advanced:
						HandleSelectedSourceCombinedAdvancedMode(source);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// In dual display mode we allow the user to select which display to route to.
		/// </summary>
		/// <param name="source"></param>
		private void HandleSelectedSourceMultiDisplay(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			IDeviceBase device = m_Room.Core.Originators.GetChild<IDeviceBase>(source.Device);
			IConferenceDeviceControl dialer = device.Controls.GetControl<IConferenceDeviceControl>();

			// Edge case - route the codec to both displays and open the context menu
			if (dialer != null && dialer.Supports.HasFlag(eCallType.Video))
			{
				// Show the context menu before routing for UX
				ConnectProRoom.FocusSource = source;
				m_Room.Routing.RouteVtc(source);
			}
			// Edge case - open the audio conferencing context menu
			else if (dialer != null && dialer.Supports.HasFlag(eCallType.Audio))
			{
				// Show the context menu before routing for UX
				ConnectProRoom.FocusSource = source;
				m_Room.Routing.RouteAtc(source);
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

			IDeviceBase device = m_Room.Core.Originators.GetChild<IDeviceBase>(source.Device);
			IConferenceDeviceControl dialer = device.Controls.GetControl<IConferenceDeviceControl>();

			// Edge case - route the codec to both displays and open the context menu
			if (dialer != null && dialer.Supports.HasFlag(eCallType.Video))
			{
				// Show the context menu before routing for UX
				ConnectProRoom.FocusSource = source;
				m_Room.Routing.RouteVtc(source);
			}
			// Edge case - open the audio conferencing context menu
			else if (dialer != null && dialer.Supports.HasFlag(eCallType.Audio))
			{
				// Show the context menu before routing for UX
				ConnectProRoom.FocusSource = source;
				m_Room.Routing.RouteAtc(source);
			}
			// Typical case - continue routing
			else
			{
				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source);
				m_Room.Routing.RouteToAllDisplays(source);
			}

			SetSelectedSource(null);
		}

		/// <summary>
		/// In combined simple mode we route the source immediately.
		/// </summary>
		/// <param name="source"></param>
		private void HandleSelectedSourceCombinedSimpleMode(ISource source)
		{
			// Same as single display, eventually might need to be unique to combined simple mode
			HandleSelectedSourceSingleDisplay(source);
		}

		/// <summary>
		/// In combined advanced mode we allow the user to select which display to route to.
		/// </summary>
		/// <param name="source"></param>
		private void HandleSelectedSourceCombinedAdvancedMode(ISource source)
		{
			// Same as dual display, eventually might need to be unique to combined advanced mode
			HandleSelectedSourceMultiDisplay(source);
		}

		/// <summary>
		/// Called to update the selection state of the given destination.
		/// </summary>
		/// <param name="routedSource"></param>
		/// <param name="destination"></param>
		private void HandleSelectedDisplay(ISource routedSource, IDestinationBase destination)
		{
			if (m_Room == null)
				return;

			if (destination == null)
				return;

			// Store in local variable because route feedback will change the field
			ISource selectedSource = m_RoutingSection.Execute(() => m_SelectedSource);

			// If no source is selected for routing then we open the contextual menu for the current routed source
			if (selectedSource == null || selectedSource == routedSource)
			{
				ShowSourceContextualMenu(routedSource);
				return;
			}

			// If a source is currently selected then we route that source to the selected display

			// Can the active source even be routed to this destination?
			if (!m_Room.Routing.HasPath(selectedSource, destination, eConnectionType.Video))
				return;

			// Reset the selection timeout
			m_SourceSelectionTimeout.Reset(SOURCE_SELECTION_TIMEOUT);

			m_Room.Routing.RouteToDisplay(selectedSource, destination);
				
			routedSource = selectedSource;

			if (ShowSourceContextualMenu(routedSource))
				SetSelectedSource(null);
		}

		private bool ShowSourceContextualMenu(ISource source)
		{
			if (m_Room == null)
				return false;

			if (source == null)
				return false;

			eControlOverride controlOverride = ConnectProRoutingSources.GetControlOverride(source);
			IDeviceControl control = ConnectProRoutingSources.GetDeviceControl(source, controlOverride);

			IContextualControlPresenter presenter = GetContextualMenu(control, controlOverride);
			if (presenter == null)
				return false;

			presenter.SetControl(control);
			presenter.ShowView(true);

			SetSelectedSource(null);

			return true;
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
				m_NavigationController.LazyLoadPresenter<IMenu2DisplaysPresenter>().SetSelectedSource(m_SelectedSource);
				m_NavigationController.LazyLoadPresenter<IMenu3PlusDisplaysPresenter>().SetSelectedSource(m_SelectedSource);
				m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>().SetSelectedSource(m_SelectedSource);
				m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>().SetSelectedSource(m_SelectedSource);

				m_SourceSelectionTimeout.Reset(SOURCE_SELECTION_TIMEOUT);
			}
			finally
			{
				m_RoutingSection.Leave();
			}
		}

		[CanBeNull]
		private IContextualControlPresenter GetContextualMenu(IDeviceControl control, eControlOverride controlOverride)
		{
			if (controlOverride == eControlOverride.Default)
				return GetContextualMenu(control);

			Type presenterType;
			if (!s_OverrideToPresenterType.TryGetValue(controlOverride, out presenterType))
				throw new ArgumentOutOfRangeException("controlOverride");

			IContextualControlPresenter presenter =
				m_NavigationController.LazyLoadPresenter<IContextualControlPresenter>(presenterType);
			if (presenter.SupportsControl(control))
				return presenter;

			if (m_Room != null)
				m_Room.Logger.AddEntry(eSeverity.Error, "Unable to use {0} context menu with {1}", controlOverride, control);
			return null;
		}

		[CanBeNull]
		private IContextualControlPresenter GetContextualMenu(IDeviceControl control)
		{
			if (control == null)
				return null;

			return s_ControlToPresenterType.Where(kvp => control.GetType().IsAssignableTo(kvp.Key))
			                               .SelectMany(kvp => kvp.Value)
			                               .Select(v => m_NavigationController.LazyLoadPresenter<IContextualControlPresenter>(v))
			                               .FirstOrDefault(v => v.SupportsControl(control));
		}
		
		/// <summary>
		/// Called when the user presses a destination in the presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedSource"></param>
		/// <param name="destination"></param>
		private void DisplaysPresenterOnDestinationPressed(object sender, ISource routedSource, IDestinationBase destination)
		{
			HandleSelectedDisplay(routedSource, destination);
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
			room.OnFocusSourceChanged += RoomOnFocusSourceChanged;

			room.Routing.State.OnDisplaySourceChanged += RoutingOnDisplaySourceChanged;
			room.Routing.State.OnAudioSourceChanged += RoutingOnAudioSourceChanged;
			room.Routing.State.OnSourceRoutedChanged += RoutingOnSourceRoutedChanged;

			ConnectProCombineRoom combineRoom = room as ConnectProCombineRoom;
			if (combineRoom == null)
				return;

			combineRoom.OnCombinedAdvancedModeChanged += CombineRoomOnCombinedAdvancedModeChanged;
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
			room.OnFocusSourceChanged -= RoomOnFocusSourceChanged;

			room.Routing.State.OnDisplaySourceChanged -= RoutingOnDisplaySourceChanged;
			room.Routing.State.OnAudioSourceChanged -= RoutingOnAudioSourceChanged;
			room.Routing.State.OnSourceRoutedChanged -= RoutingOnSourceRoutedChanged;

			ConnectProCombineRoom combineRoom = room as ConnectProCombineRoom;
			if (combineRoom == null)
				return;

			combineRoom.OnCombinedAdvancedModeChanged -= CombineRoomOnCombinedAdvancedModeChanged;
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
			{
				m_Room.Routing.State.ClearProcessingSources();
				m_Room.Routing.State.ClearMaskedSources();
			}

			UpdateMeetingPresentersVisibility();
		}

		/// <summary>
		/// Called the
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnFocusSourceChanged(object sender, SourceEventArgs eventArgs)
		{
			if (eventArgs.Data != null)
			{
				ShowSourceContextualMenu(eventArgs.Data);
				return;
			}

			foreach (Type controlPresenterType in s_OverrideToPresenterType.Values)
				m_NavigationController.LazyLoadPresenter(controlPresenterType).ShowView(false);
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

		private void CombineRoomOnCombinedAdvancedModeChanged(object sender, CombineAdvancedModeEventArgs eventArgs)
		{
			SetSelectedSource(null);
			UpdateMeetingPresentersVisibility();

			UpdateRouting();
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

			ConnectProCombineRoom combineRoom = m_Room as ConnectProCombineRoom;

			bool combinedRoom = combineRoom != null;
			bool dualDisplays = m_Room != null && !combinedRoom && m_Room.Routing.Destinations.DisplayDestinationsCount == 2;
			bool manyDisplays = m_Room != null && !combinedRoom && m_Room.Routing.Destinations.DisplayDestinationsCount > 2;
			bool combineAdvanced = combineRoom != null && combineRoom.CombinedAdvancedMode == eCombineAdvancedMode.Advanced;
			bool combineSimple = combineRoom != null && combineRoom.CombinedAdvancedMode == eCombineAdvancedMode.Simple;

			m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().ShowView(true);
			m_NavigationController.LazyLoadPresenter<IMenu2DisplaysPresenter>().ShowView(dualDisplays);
			m_NavigationController.LazyLoadPresenter<IMenu3PlusDisplaysPresenter>().ShowView(manyDisplays);
			m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>().ShowView(combineAdvanced);
			m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>().ShowView(combineSimple);
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
			if (m_Room == null)
				return;

			ConnectProCombineRoom combineRoom = m_Room as ConnectProCombineRoom;

			// Perf - Don't bother updating destination routing if there is only 1 destination in the system
			if (combineRoom == null && m_Room.Routing.Destinations.DisplayDestinationsCount < 2)
				return;

			IcdHashSet<ISource> activeAudio =
				m_Room.Routing
				      .State
				      .GetCachedActiveAudioSources()
				      .ToIcdHashSet();

			Dictionary<IDestinationBase, IcdHashSet<ISource>> activeVideo =
				m_Room.Routing
				      .State
				      .GetFakeActiveVideoSources()
				      .ToDictionary();

			if (combineRoom == null)
				UpdateDisplaysRouting(activeVideo, activeAudio);
			else
				UpdateCombinedDisplaysRouting(activeVideo, activeAudio);

			// If the active source is routed to all destinations we clear the active source
			if (m_SelectedSource != null && activeVideo.All(kvp => kvp.Value.Contains(m_SelectedSource)))
				SetSelectedSource(null);
		}

		private void UpdateDisplaysRouting(IDictionary<IDestinationBase, IcdHashSet<ISource>> activeVideo,
		                                   IcdHashSet<ISource> activeAudio)
		{
			int displayCount = m_Room == null ? 0 : m_Room.Routing.Destinations.DisplayDestinationsCount;
			if (displayCount <= 1)
				return;

			if (displayCount >= 3)
				m_NavigationController.LazyLoadPresenter<IMenu3PlusDisplaysPresenter>()
				                      .SetRouting(activeVideo, activeAudio);
			else
				m_NavigationController.LazyLoadPresenter<IMenu2DisplaysPresenter>()
				                      .SetRouting(activeVideo, activeAudio);
		}

		private void UpdateCombinedDisplaysRouting(IDictionary<IDestinationBase, IcdHashSet<ISource>> activeVideo,
		                                           IcdHashSet<ISource> activeAudio)
		{
			ConnectProCombineRoom combineRoom = m_Room as ConnectProCombineRoom;
			if (combineRoom == null)
				return;

			switch (combineRoom.CombinedAdvancedMode)
			{
				case eCombineAdvancedMode.Simple:
					m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>()
					                      .SetRouting(activeVideo, activeAudio);
					break;

				case eCombineAdvancedMode.Advanced:
					m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>()
					                      .SetRouting(activeVideo, activeAudio);
					m_NavigationController.LazyLoadPresenter<IMenuRouteSummaryPresenter>()
					                      .SetRouting(activeVideo);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion

		#region Theme Callbacks

		/// <summary>
		/// Subscribe to the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Subscribe(ConnectProTheme theme)
		{
			theme.OnStartRoomCombine += ThemeOnStartRoomCombine;
			theme.OnEndRoomCombine += ThemeOnEndRoomCombine;
		}

		/// <summary>
		/// Unsubscribe from the theme events.
		/// </summary>
		/// <param name="theme"></param>
		private void Unsubscribe(ConnectProTheme theme)
		{
			theme.OnStartRoomCombine -= ThemeOnStartRoomCombine;
			theme.OnEndRoomCombine -= ThemeOnEndRoomCombine;
		}

		/// <summary>
		/// Called when the theme starts combining rooms.
		/// </summary>
		/// <param name="connectProTheme"></param>
		/// <param name="openCount"></param>
		/// <param name="closeCount"></param>
		private void ThemeOnStartRoomCombine(ConnectProTheme connectProTheme, int openCount, int closeCount)
		{
			string message = "Updating Configuration";

			if (openCount == 0)
				message = "Uncombining Rooms";
			else if (closeCount == 0)
				message = "Combining Rooms";

			m_NavigationController.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>()
			                      .ShowView(message);
		}

		/// <summary>
		/// Called when the theme finishes combining rooms.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ThemeOnEndRoomCombine(object sender, GenericEventArgs<Exception> eventArgs)
		{
			if (eventArgs.Data == null)
				m_NavigationController.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>()
				                      .ShowView(false);
			else
				m_NavigationController.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>()
				                      .TimeOut("Failed to complete operation - " + eventArgs.Data.Message);
		}

		#endregion

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		private void SubscribePresenters()
		{
			Subscribe(m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>());

			Subscribe(m_NavigationController.LazyLoadPresenter<IMenu2DisplaysPresenter>());
			Subscribe(m_NavigationController.LazyLoadPresenter<IMenu3PlusDisplaysPresenter>());
			Subscribe(m_NavigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>());
			Subscribe(m_NavigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>());
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		private void UnsubscribePresenters()
		{
			Unsubscribe(m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>());

			Unsubscribe(m_NavigationController.LazyLoadPresenter<IMenu2DisplaysPresenter>());
			Unsubscribe(m_NavigationController.LazyLoadPresenter<IMenu3PlusDisplaysPresenter>());
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

		#region Menu 2 Displays Callbacks

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IMenu2DisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed += DisplaysPresenterOnDestinationPressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IMenu2DisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed -= DisplaysPresenterOnDestinationPressed;
		}

		#endregion

		#region Menu 3+ Displays Callbacks

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IMenu3PlusDisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed += DisplaysPresenterOnDestinationPressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IMenu3PlusDisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed -= DisplaysPresenterOnDestinationPressed;
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
			ConnectProCombineRoom combineRoom = m_Room as ConnectProCombineRoom;
			if (combineRoom != null)
				combineRoom.CombinedAdvancedMode = eCombineAdvancedMode.Simple;
		}

		private void PresenterOnAdvancedModePressed(object sender, EventArgs e)
		{
			ConnectProCombineRoom combineRoom = m_Room as ConnectProCombineRoom;
			if (combineRoom != null)
				combineRoom.CombinedAdvancedMode = eCombineAdvancedMode.Advanced;
		}

		#endregion
	}
}
