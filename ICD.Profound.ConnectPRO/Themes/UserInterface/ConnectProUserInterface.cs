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
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Sources.TvTuner.Controls;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
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
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VisibilityTree;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class ConnectProUserInterface : IUserInterface
	{
		private const long SOURCE_SELECTION_TIMEOUT = 8 * 1000;

		private readonly IPanelDevice m_Panel;
		private readonly INavigationController m_NavigationController;
		private readonly SafeTimer m_SourceSelectionTimeout;

		private ISource m_ProcessingSource; 

		private IConnectProRoom m_Room;
		private DefaultVisibilityNode m_RootVisibility;
		private ISource m_ActiveSource;

		#region Properties

		public IPanelDevice Panel { get { return m_Panel; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProUserInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			m_Panel = panel;
			UpdatePanelOnlineJoin();

			m_SourceSelectionTimeout = SafeTimer.Stopped(() => SetActiveSource(null));

			IViewFactory viewFactory = new ConnectProViewFactory(panel, theme);
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

			// Video Conference node
			IVisibilityNode videoConferencingVisibility = new SingleVisibilityNode();
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcContactsPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcCameraPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcSharePresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcDtmfPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcActiveCallsPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcKeyboardPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcKeypadPresenter>());

			// Audio Conference node
			IVisibilityNode audioConferencingVisibility = new SingleVisibilityNode();
			audioConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IAtcBasePresenter>());

			// Meeting node
			IVisibilityNode meetingVisibility = new VisibilityNode();

			meetingVisibility.AddNode(videoConferencingVisibility);
			meetingVisibility.AddNode(audioConferencingVisibility);

			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IEndMeetingPresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOptionPrivacyMutePresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOptionVolumePresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOptionCameraPresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcBasePresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ICableTvPresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWebConferencingAlertPresenter>());
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWebConferencingStepPresenter>());

			m_RootVisibility.AddNode(meetingVisibility);

			// Settings node
			IVisibilityNode settingsVisibility = new SingleVisibilityNode();
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsPasscodePresenter>());
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsSystemPowerPresenter>());

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

			UpdateMeetingPresenters();
			UpdateRouting();

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
			if (m_Room == null)
				return;

			// If the source is already active then clear the active state
			if (source == m_ActiveSource)
			{
				SetActiveSource(null);
				return;
			}

			bool dualDisplays = m_Room.Routing.IsDualDisplayRoom;

			// In a dual display room we allow the user to select which display to route to
			if (dualDisplays)
			{
				IDialingDeviceControl videoDialer = m_Room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
				IDialingDeviceControl audioDialer = m_Room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Audio);

				// Edge case - route the codec to both displays and open the context menu
				if (source != null && videoDialer != null && source.Device == videoDialer.Parent.Id)
				{
					IRouteSourceControl sourceControl = m_Room.Core.GetControl<IRouteSourceControl>(source.Device, source.Control);
					m_Room.Routing.RouteVtc(sourceControl);
				}
				// Edge case - open the audio conferencing context menu
				else if (source != null && audioDialer != null && source.Device == audioDialer.Parent.Id)
				{
					IRouteSourceControl sourceControl = m_Room.Core.GetControl<IRouteSourceControl>(source.Device, source.Control);
					m_Room.Routing.RouteAtc(sourceControl);	
				}
				// Typical case - continue routing
				else
				{
					SetActiveSource(source);
					return;
				}
				
				m_SourceSelectionTimeout.Reset(SOURCE_SELECTION_TIMEOUT);
			}
			// In a single display room just route the source immediately
			else
			{
				if (source != null)
				{
					SetProcessingSource(source);

					m_Room.Routing.Route(source);
					m_Room.Routing.RouteAudio(source);
				}

				SetActiveSource(null);
			}

			ShowSourceContextualMenu(source, false);
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
			ISource activeSource = m_ActiveSource;

			// If no source is selected for routing then we open the contextual menu for the current routed source
			if (activeSource == null)
			{
				ShowSourceContextualMenu(routedSource, false);
			}
			// If a source is currently selected then we route that source to the selected display
			else
			{
				SetProcessingSource(activeSource);

				m_Room.Routing.Route(activeSource, destination);
				m_Room.Routing.RouteAudioIfUnrouted(activeSource);
				routedSource = activeSource;

				if (ShowSourceContextualMenu(routedSource, true))
					SetActiveSource(null);
			}
		}

		private void SetProcessingSource(ISource source)
		{
			if (source == m_ProcessingSource)
				return;

			m_ProcessingSource = source;

			UpdateRouting();
		}

		private bool ShowSourceContextualMenu(ISource source, bool vtcOnly)
		{
			if (m_Room == null)
				return false;

			if (source == null)
				return false;

			eControlOverride controlOverride = m_Room.Routing.GetControlOverride(source);
			IDeviceControl control = m_Room.Routing.GetDeviceControl(source, controlOverride);

			if (control is IDialingDeviceControl)
			{
				IDialingDeviceControl dialer = control as IDialingDeviceControl;

				if (dialer.Supports.HasFlag(eConferenceSourceType.Video))
					m_NavigationController.NavigateTo<IVtcBasePresenter>();
				else
					m_NavigationController.NavigateTo<IAtcBasePresenter>();

				return true;
			}

			if (vtcOnly)
				return false;

			if (control is ITvTunerControl)
			{
				m_NavigationController.NavigateTo<ICableTvPresenter>().Control = control as ITvTunerControl;
				return true;
			}

			switch (controlOverride)
			{
				case eControlOverride.WebConference:
					m_NavigationController.NavigateTo<IWebConferencingAlertPresenter>();
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Sets the source that is currently active for routing to the displays.
		/// </summary>
		/// <param name="source"></param>
		private void SetActiveSource(ISource source)
		{
			if (source == m_ActiveSource)
				return;

			m_ActiveSource = source;

			m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().ActiveSource = m_ActiveSource;
			m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>().ActiveSource = m_ActiveSource;

			m_SourceSelectionTimeout.Reset(SOURCE_SELECTION_TIMEOUT);
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
			room.Routing.OnDisplaySourceChanged += RoutingOnDisplaySourceChanged;
			room.Routing.OnAudioSourceChanged += RoutingOnAudioSourceChanged;
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
			room.Routing.OnDisplaySourceChanged -= RoutingOnDisplaySourceChanged;
			room.Routing.OnAudioSourceChanged -= RoutingOnAudioSourceChanged;
		}

		/// <summary>
		/// Called when the room enters/exits a meeting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateMeetingPresenters();
		}

		/// <summary>
		/// Sets the visibility of the subpages based on the meeting state.
		/// </summary>
		private void UpdateMeetingPresenters()
		{
			bool isInMeeting = m_Room != null && m_Room.IsInMeeting;

			// Set the visibility of the meeting buttons
			m_NavigationController.LazyLoadPresenter<IStartMeetingPresenter>().ShowView(!isInMeeting);
			m_NavigationController.LazyLoadPresenter<IEndMeetingPresenter>().ShowView(isInMeeting);

			bool dualDisplays = m_Room != null && m_Room.Routing.IsDualDisplayRoom;

			// Set the visibility of the source a display subpages
			bool displaysVisible = isInMeeting && dualDisplays;

			m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().ShowView(true);
			m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>().ShowView(displaysVisible);
		}

		/// <summary>
		/// Update the UI with the active audio sources.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoutingOnAudioSourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateRouting();
		}

		/// <summary>
		/// Update the UI with the active video sources.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoutingOnDisplaySourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateRouting();
		}

		/// <summary>
		/// Updates the routing state in the UI.
		/// </summary>
		private void UpdateRouting()
		{
			IcdHashSet<ISource> activeAudio =
				(m_Room == null
					 ? Enumerable.Empty<ISource>()
					 : m_Room.Routing
					         .GetCachedActiveAudioSources())
					.ToIcdHashSet();

			Dictionary<IDestination, IcdHashSet<ISource>> routing =
				(m_Room == null
					 ? Enumerable.Empty<KeyValuePair<IDestination, IcdHashSet<ISource>>>()
					 : m_Room.Routing
					         .GetCachedActiveVideoSources())
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			Dictionary<ISource, eRoutedState> routedSources =
				routing.Values
				       .SelectMany(v => v)
				       .Distinct()
				       .ToDictionary(s => s, s => eRoutedState.Active);

			// Remove routed items from the processing sources collection
			foreach (ISource source in routing.Values.SelectMany(v => v))
			{
				if (source == m_ProcessingSource)
					m_ProcessingSource = null;
			}

			if (m_ProcessingSource != null)
				routedSources.Add(m_ProcessingSource, eRoutedState.Processing);

			m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().SetRoutedSources(routedSources);
			m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>().SetRouting(routing, activeAudio);

			// If the active source is routed to all destinations we clear the active source
			if (m_ActiveSource != null && routing.All(kvp => kvp.Value.Contains(m_ActiveSource)))
				SetActiveSource(null);
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
			presenter.OnDestinationPressed += MenuDisplaysPresenterOnDestinationPressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IMenuDisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed -= MenuDisplaysPresenterOnDestinationPressed;
		}

		/// <summary>
		/// Called when the user presses a destination in the presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="routedSource"></param>
		/// <param name="destination"></param>
		private void MenuDisplaysPresenterOnDestinationPressed(object sender, ISource routedSource, IDestination destination)
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

		private void VtcIncomingCallPresenterOnCallAnswered(object sender, EventArgs e)
		{
			if (m_Room == null)
				return;

			IDialingDeviceControl videoDialer = m_Room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
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

		private void AtcIncomingCallPresenterOnCallAnswered(object sender, EventArgs e)
		{
			if (m_Room == null)
				return;

			IDialingDeviceControl audioDialer = m_Room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Audio);
			IDeviceBase device = audioDialer == null ? null : audioDialer.Parent;

			if (device == null)
				return;

			ISource source = m_Room.Originators.GetInstancesRecursive<ISource>(s => s.Device == device.Id).FirstOrDefault();
			if (source == null)
				return;

			HandleSelectedSource(source);
		}

		#endregion
	}
}
