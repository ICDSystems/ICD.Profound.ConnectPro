using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Settings.Originators;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Rooms.Single;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Background;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface
{
	public sealed class ConnectProTouchDisplayInterface : AbstractUserInterface
	{
		private static readonly Dictionary<eControlOverride, Type> s_OverrideToPresenterType =
			new Dictionary<eControlOverride, Type>
			{
			};

		/// <summary>
		/// Maps control types to potential context menu presenter types.
		/// Checks each presenter type for compatibility and falls back to the next.
		/// </summary>
		private static readonly Dictionary<Type, List<Type>> s_ControlToPresenterType =
			new Dictionary<Type, List<Type>>
			{
				{typeof(IConferenceDeviceControl), new List<Type> {typeof(IConferenceBasePresenter)}},
			};

		private readonly ITouchDisplayNavigationController m_NavigationController;

		private IVisibilityNode m_DefaultNotification;
		private IVisibilityNode m_MainPageVisibility;
		private IVisibilityNode m_NotificationVisibility;

		private IConnectProRoom m_Room;
		private bool m_UserInterfaceReady;

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProTouchDisplayInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			Panel = panel;
			UpdatePanelOfflineJoin();

			ITouchDisplayViewFactory viewFactory = new ConnectProTouchDisplayViewFactory(panel, theme);
			m_NavigationController = new ConnectProTouchDisplayNavigationController(viewFactory, theme);

			BuildVisibilityTree();
		}

		/// <summary>
		///     Release resources.
		/// </summary>
		public override void Dispose()
		{
			SetRoom(null);

			m_NavigationController.Dispose();
		}

		/// <summary>
		///     Updates the "offline" visual state of the panel
		/// </summary>
		private void UpdatePanelOfflineJoin()
		{
			Panel.SendInputDigital(CommonJoins.DIGITAL_OFFLINE_JOIN, m_Room == null || !m_UserInterfaceReady);
		}

		/// <summary>
		///     Builds the rules for view visibility, e.g. prevent certain items from being visible at the same time.
		/// </summary>
		private void BuildVisibilityTree()
		{
			//// Show "hello" when no notifications are visible
			//m_DefaultNotification = new NotificationVisibilityNode(m_NavigationController.LazyLoadPresenter<IOsdHelloPresenter>());
			//m_DefaultNotification.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdIncomingCallPresenter>());
			//m_DefaultNotification.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdMutePresenter>());

			//// show "welcome" when no other main page is visible
			m_MainPageVisibility = new SingleVisibilityNode();
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<ISchedulePresenter>());
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IDeviceDrawerPresenter>());
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IStartConferencePresenter>());

			//// these presenters are initially visible
			//m_NavigationController.NavigateTo<IOsdHelloPresenter>();

			// always visible
			m_NavigationController.NavigateTo<IHeaderPresenter>();
			m_NavigationController.LazyLoadPresenter<IBackgroundPresenter>();

			UpdateVisibility();
		}

		#region Properties

		public IPanelDevice Panel { get; }

		public override IRoom Room { get { return ConnectProRoom; } }

		public IConnectProRoom ConnectProRoom { get { return m_Room; } }

		public override object Target => Panel;

		#endregion

		#region Methods

		/// <summary>
		///     Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IRoom room)
		{
			SetRoom(room as IConnectProRoom);
		}

		/// <summary>
		///     Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			if (room == m_Room)
				return;

			ServiceProvider.GetService<ILoggerService>()
				.AddEntry(eSeverity.Informational, "{0} setting room to {1}", this, room);

			Unsubscribe(m_Room);
			m_Room = room;
			Subscribe(m_Room);

			m_NavigationController.SetRoom(room);
			
			UpdateRoutedSources();

			UpdateVisibility();
			UpdatePanelOfflineJoin();
		}

		/// <summary>
		///     Tells the UI that it should be considered ready to use.
		///     For example updating the online join on a panel or starting a long-running process that should be delayed.
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

			HandleSelectedSourceSingleDisplay(source);
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

			if (!m_Room.Routing.State.GetIsRoutedCached(source, eConnectionType.Video))
				m_Room.Routing.State.SetProcessingSource(source);

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
				ShowSourceContextualMenu(source);
				m_Room.Routing.RouteAtc(source);
			}
			// Typical case - continue routing
			else
			{
				// Show the context menu before routing for UX
				ShowSourceContextualMenu(source);
				m_Room.Routing.RouteAllDisplays(source);
			}
			
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

			return true;
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

		#endregion

		#region Room Callbacks

		/// <summary>
		///     Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Subscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
			room.OnFocusSourceChanged += RoomOnFocusSourceChanged;

			room.Routing.State.OnSourceRoutedChanged += RoutingStateOnSourceRoutedChanged;
		}

		/// <summary>
		///     Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
			room.OnFocusSourceChanged -= RoomOnFocusSourceChanged;

			room.Routing.State.OnSourceRoutedChanged -= RoutingStateOnSourceRoutedChanged;
		}

		private void RoutingStateOnSourceRoutedChanged(object sender, EventArgs e)
		{
			UpdateRoutedSources();
			UpdateVisibility();
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs eventArgs)
		{
			if (m_Room != null)
			{
				m_Room.Routing.State.ClearProcessingSources();
				m_Room.Routing.State.ClearMaskedSources();
			}

			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			if (Room == null)
				return; 
			IEnumerable<ISource> activeSources = m_Room.Routing.State.GetSourceRoutedStates()
				.Where(kvp => kvp.Value == eSourceState.Masked || kvp.Value == eSourceState.Active)
				.Select(kvp => kvp.Key).ToList();
			var zoomRouted = false;
			foreach (var source in activeSources)
			{
				IOriginator child;
				if (Room.Core.Originators.TryGetChild(source.Device, out child) && child is ZoomRoom)
				{
					zoomRouted = true;
					var control = (child as IDevice).Controls.GetControl<IConferenceDeviceControl>();
					m_NavigationController.LazyLoadPresenter<IStartConferencePresenter>().ActiveConferenceControl = (IWebConferenceDeviceControl)control;
					break;
				}
			}

			if (zoomRouted)
				m_NavigationController.NavigateTo<IStartConferencePresenter>();
			else if (m_Room.IsInMeeting)
				m_NavigationController.NavigateTo<IDeviceDrawerPresenter>();
			else if (m_Room.CalendarControl != null)
				m_NavigationController.NavigateTo<ISchedulePresenter>();
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

		private void UpdateRoutedSources()
		{
			Dictionary<ISource, eSourceState> routedSources =
				m_Room == null
					? new Dictionary<ISource, eSourceState>()
					: m_Room.Routing.State.GetSourceRoutedStates().ToDictionary();

			m_NavigationController.LazyLoadPresenter<IDeviceDrawerPresenter>().SetRoutedSources(routedSources);
		}

		#endregion

		#region Source Select Callbacks

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IDeviceDrawerPresenter presenter)
		{
			presenter.OnSourcePressed += SourceSelectPresenterOnSourcePressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IDeviceDrawerPresenter presenter)
		{
			presenter.OnSourcePressed -= SourceSelectPresenterOnSourcePressed;
		}

		/// <summary>
		/// Called when the user presses a source in the presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="source"></param>
		private void SourceSelectPresenterOnSourcePressed(object sender, SourceEventArgs args)
		{
			if (args != null && args.Data != null)
				HandleSelectedSource(args.Data);
		}

		#endregion
	}
}