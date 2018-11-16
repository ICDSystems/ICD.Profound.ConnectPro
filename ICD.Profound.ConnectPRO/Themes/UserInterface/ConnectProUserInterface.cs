﻿using System;
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
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VisibilityTree;
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

		private readonly IcdHashSet<ISource> m_ActiveAudio;
		private readonly Dictionary<IDestination, IcdHashSet<ISource>> m_ActiveVideo;
		private readonly Dictionary<ISource, eSourceState> m_SourceRoutedStates;
		private readonly Dictionary<IDestination, ISource> m_ProcessingSources;
		private readonly SafeCriticalSection m_RoutingSection;

		private IConnectProRoom m_Room;
		private DefaultVisibilityNode m_RootVisibility;
		private ISource m_ActiveSource;

		#region Properties

		public IPanelDevice Panel { get { return m_Panel; } }

		public IConnectProRoom Room { get { return m_Room; } }

		object IUserInterface.Target { get { return Panel; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProUserInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			m_ActiveAudio = new IcdHashSet<ISource>();
			m_ActiveVideo = new Dictionary<IDestination, IcdHashSet<ISource>>();
			m_SourceRoutedStates = new Dictionary<ISource, eSourceState>();
			m_ProcessingSources = new Dictionary<IDestination, ISource>();
			m_RoutingSection = new SafeCriticalSection();

			m_Panel = panel;
			UpdatePanelOnlineJoin();

			m_SourceSelectionTimeout = SafeTimer.Stopped(() => SetActiveSource(null));

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

			// Video Conference node
			IVisibilityNode videoConferencingVisibility = new SingleVisibilityNode();
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcContactsNormalPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcContactsPolycomPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcCameraPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcSharePresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcDtmfPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcActiveCallsPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcKeyboardPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcKeypadPresenter>());
			
			// Web Conference node
			IVisibilityNode wtcMeetingVisibility = new SingleVisibilityNode();
			wtcMeetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcCallOutPresenter>());
			wtcMeetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcSharePresenter>());
			wtcMeetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcRecordingPresenter>());
			wtcMeetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcActiveMeetingPresenter>());

			IVisibilityNode wtcMainVisibility = new SingleVisibilityNode();
			wtcMainVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcButtonListPresenter>());
			wtcMainVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcMainPagePresenter>());

			IVisibilityNode wtcToggleVisibility = new SingleVisibilityNode();
			wtcToggleVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcActiveMeetingTogglePresenter>());
			wtcToggleVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IWtcContactsTogglePresenter>());

			IVisibilityNode webConferencingVisibility = new VisibilityNode();
			webConferencingVisibility.AddNode(wtcMeetingVisibility);
			webConferencingVisibility.AddNode(wtcMainVisibility);
			webConferencingVisibility.AddNode(wtcToggleVisibility);

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

			// Settings node
			IVisibilityNode settingsVisibility = new SingleVisibilityNode();
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsPasscodePresenter>());
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsSystemPowerPresenter>());
			settingsVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISettingsDirectoryPresenter>());

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
			UpdateRouting(EnumUtils.GetFlagsAllValue<eConnectionType>());

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
				if (source == m_ActiveSource)
				{
					SetActiveSource(null);
					return;
				}
			}
			finally
			{
				m_RoutingSection.Leave();
			}

			if (m_Room.Routing.IsDualDisplayRoom)
				HandleSelectedSourceDualDisplay(source);
			else
				HandleSelectedSourceSingleDisplay(source);
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
				SetActiveSource(source);
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
			if (m_RoutingSection.Execute(() => m_ActiveVideo.Any(kvp => kvp.Value.Contains(source))))
			{
				ShowSourceContextualMenu(source, false);
			}
			else
			{
				SetProcessingSource(source);

				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source, false);

				m_Room.Routing.RouteSingleDisplay(source);
			}

			SetActiveSource(null);
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
			ISource activeSource = m_RoutingSection.Execute(() => m_ActiveSource);

			// If no source is selected for routing then we open the contextual menu for the current routed source
			if (activeSource == null || activeSource == routedSource)
			{
				ShowSourceContextualMenu(routedSource, false);
			}
			// If a source is currently selected then we route that source to the selected display
			else if (activeSource != routedSource)
			{
				bool overrideAudio;

				m_RoutingSection.Enter();

				try
				{
					SetProcessingSource(destination, activeSource);
					overrideAudio = CanOverrideAudio(activeSource, destination);
				}
				finally
				{
					m_RoutingSection.Leave();
				}

				m_Room.Routing.RouteDualDisplay(activeSource, destination, overrideAudio);
				
				routedSource = activeSource;

				if (ShowSourceContextualMenu(routedSource, true))
					SetActiveSource(null);
			}
		}

		/// <summary>
		/// Returns true if we can override active audio when routing the given source to the given destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		private bool CanOverrideAudio(ISource source, IDestination destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			m_RoutingSection.Enter();

			try
			{
				// Nothing is currently routed for audio
				if (m_ActiveAudio.Count == 0)
					return false;

				// If there are no sources routed to the display then there is nothing to override
				IcdHashSet<ISource> oldSources;
				if (!m_ActiveVideo.TryGetValue(destination, out oldSources) || oldSources.Count == 0)
					return false;

				// No change
				if (oldSources.Contains(source))
					return false;

				// Old source is not current active audio
				if (!m_ActiveAudio.Intersect(oldSources).Any())
					return false;

				// Is there another source routed for audio going to another display?
				foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> kvp in m_ActiveVideo)
				{
					// Skip the display we are currently routing to
					IDestination display = kvp.Key;
					if (display == destination)
						continue;

					// Are we in the middle of routing a new source to the display?
					ISource processing = m_ProcessingSources.GetDefault(display);
					if (processing != null)
						continue;

					// The display has a source that is being listened to
					if (kvp.Value.Intersect(m_ActiveAudio).Any())
						return false;
				}

				return true;
			}
			finally
			{
				m_RoutingSection.Leave();
			}
		}

		/// <summary>
		/// Sets the processing source for the single display destination.
		/// </summary>
		/// <param name="source"></param>
		private void SetProcessingSource(ISource source)
		{
			IDestination destination = m_Room == null ? null : m_Room.Routing.GetDisplayDestinations().FirstOrDefault();
			if (destination == null)
				return;

			SetProcessingSource(destination, source);
		}

		/// <summary>
		/// Sets the processing source for the given destination.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="source"></param>
		private void SetProcessingSource(IDestination destination, ISource source)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			m_RoutingSection.Enter();

			try
			{
				// No change
				if (source == m_ProcessingSources.GetDefault(destination))
					return;

				// Is the source already routed to the destination?
				IcdHashSet<ISource> routed;
				if (m_ActiveVideo.TryGetValue(destination, out routed) && routed.Contains(source))
					return;

				m_ProcessingSources[destination] = source;

				UpdateSourceRoutedStates();
			}
			finally
			{
				m_RoutingSection.Leave();
			}
		}

		private bool ShowSourceContextualMenu(ISource source, bool vtcOnly)
		{
			if (m_Room == null)
				return false;

			if (source == null)
				return false;

			eControlOverride controlOverride = ConnectProRouting.GetControlOverride(source);
			IDeviceControl control = ConnectProRouting.GetDeviceControl(source, controlOverride);

			if (control is IConferenceDeviceControl)
			{
				ITraditionalConferenceDeviceControl dialer = control as ITraditionalConferenceDeviceControl;

				if (dialer != null)
				{
					if (dialer.Supports.HasFlag(eCallType.Video))
						m_NavigationController.NavigateTo<IVtcBasePresenter>();
					else
						m_NavigationController.NavigateTo<IAtcBasePresenter>();
				}

				IWebConferenceDeviceControl webControl = control as IWebConferenceDeviceControl;
				if (webControl != null)
					m_NavigationController.NavigateTo<IWtcBasePresenter>();

				SetActiveSource(null);
				return true;
			}

			if (vtcOnly)
				return false;

			if (control is ITvTunerControl)
			{
				m_NavigationController.NavigateTo<ICableTvPresenter>().Control = control as ITvTunerControl;
				SetActiveSource(null);
				return true;
			}

			switch (controlOverride)
			{
				case eControlOverride.WebConference:
					m_NavigationController.NavigateTo<IWebConferencingAlertPresenter>();
					SetActiveSource(null);
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
			m_RoutingSection.Enter();

			try
			{
				if (source == m_ActiveSource)
					return;

				m_ActiveSource = source;

				m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().ActiveSource = m_ActiveSource;
				m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>().ActiveSource = m_ActiveSource;

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
			SetActiveSource(null);

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

			bool dualDisplays = m_Room != null && m_Room.Routing.IsDualDisplayRoom;

			m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().ShowView(true);
			m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>().ShowView(dualDisplays);
		}

		/// <summary>
		/// Update the UI with the active audio sources.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoutingOnAudioSourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateRouting(eConnectionType.Audio);
		}

		/// <summary>
		/// Update the UI with the active video sources.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoutingOnDisplaySourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateRouting(eConnectionType.Video);
		}

		/// <summary>
		/// Updates the routing state in the UI.
		/// </summary>
		/// <param name="type"></param>
		private void UpdateRouting(eConnectionType type)
		{
			m_RoutingSection.Enter();

			try
			{
				bool audioChange = false;
				bool videoChange = false;

				if (type.HasFlag(eConnectionType.Audio))
					audioChange = UpdateActiveAudio();

				if (type.HasFlag(eConnectionType.Video))
					videoChange = UpdateActiveVideo();

				if (audioChange || videoChange)
					m_NavigationController.LazyLoadPresenter<IMenuDisplaysPresenter>().SetRouting(m_ActiveVideo, m_ActiveAudio);
			}
			finally
			{
				m_RoutingSection.Leave();
			}
		}

		/// <summary>
		/// Builds the set of active audio sources.
		/// </summary>
		/// <returns>True if the active audio sources changed.</returns>
		private bool UpdateActiveAudio()
		{
			m_RoutingSection.Enter();

			try
			{
				IcdHashSet<ISource> activeAudio =
					(m_Room == null
						 ? Enumerable.Empty<ISource>()
						 : m_Room.Routing
						         .GetCachedActiveAudioSources())
						.ToIcdHashSet();

				if (activeAudio.SetEquals(m_ActiveAudio))
					return false;

				m_ActiveAudio.Clear();
				m_ActiveAudio.AddRange(activeAudio);

				return true;
			}
			finally
			{
				m_RoutingSection.Leave();
			}
		}

		/// <summary>
		/// Builds the map of destinations to active video sources.
		/// </summary>
		/// <returns>True if the active video sources changed.</returns>
		private bool UpdateActiveVideo()
		{
			m_RoutingSection.Enter();

			try
			{
				Dictionary<IDestination, IcdHashSet<ISource>> routing =
					(m_Room == null
						 ? Enumerable.Empty<KeyValuePair<IDestination, IcdHashSet<ISource>>>()
						 : m_Room.Routing
						         .GetCachedActiveVideoSources())
						.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

				if (routing.DictionaryEqual(m_ActiveVideo, (a, b) => a.SetEquals(b)))
					return false;

				m_ActiveVideo.Clear();
				m_ActiveVideo.AddRange(routing.Keys, k => new IcdHashSet<ISource>(routing[k]));

				// Remove routed items from the processing sources collection
				foreach (KeyValuePair<IDestination, IcdHashSet<ISource>> kvp in m_ActiveVideo)
				{
					ISource processing = m_ProcessingSources.GetDefault(kvp.Key);
					if (processing == null)
						continue;

					if (kvp.Value.Contains(processing))
						m_ProcessingSources.Remove(kvp.Key);
				}

				// If the active source is routed to all destinations we clear the active source
				if (m_ActiveSource != null && m_ActiveVideo.All(kvp => kvp.Value.Contains(m_ActiveSource)))
					SetActiveSource(null);

				UpdateSourceRoutedStates();

				return true;
			}
			finally
			{
				m_RoutingSection.Leave();
			}
		}

		private bool UpdateSourceRoutedStates()
		{
			m_RoutingSection.Enter();

			try
			{
				// Build a map of video sources to their routed state
				Dictionary<ISource, eSourceState> routedSources =
					m_ActiveVideo.Values
								 .SelectMany(v => v)
								 .Distinct()
								 .ToDictionary(s => s, s => eSourceState.Active);

				// A source may be processing for another display, so we override
				foreach (ISource source in m_ProcessingSources.Values.Where(s => s != null))
					routedSources[source] = eSourceState.Processing;

				if (routedSources.DictionaryEqual(m_SourceRoutedStates))
					return false;

				m_SourceRoutedStates.Clear();
				m_SourceRoutedStates.AddRange(routedSources);

				m_NavigationController.LazyLoadPresenter<ISourceSelectPresenter>().SetRoutedSources(m_SourceRoutedStates);

				return true;
			}
			finally
			{
				m_RoutingSection.Leave();
			}
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
	}
}
