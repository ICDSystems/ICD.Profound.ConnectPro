using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Devices;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Background;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface
{
    public sealed class ConnectProTouchDisplayInterface : AbstractUserInterface
    {
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
            //m_MainPageVisibility = new SingleVisibilityNode();
            //m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdWelcomePresenter>());
            //m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdSourcesPresenter>());
            //m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdConferencePresenter>());

            //// these presenters are initially visible
            //m_NavigationController.NavigateTo<IOsdHelloPresenter>();

            // always visible
            m_NavigationController.NavigateTo<IHeaderPresenter>();
            m_NavigationController.LazyLoadPresenter<IBackgroundPresenter>();

            UpdateVisibility();
        }

        #region Properties

        public IPanelDevice Panel { get; }

        public override IRoom Room => m_Room;

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

            UpdatePanelOfflineJoin();
            UpdateVisibility();
        }

        private void RoutingStateOnSourceRoutedChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
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
            room.Routing.State.OnSourceRoutedChanged -= RoutingStateOnSourceRoutedChanged;
        }

        private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs eventArgs)
        {
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
                    //m_NavigationController.LazyLoadPresenter<IOsdConferencePresenter>().ActiveConferenceControl = control;
                    break;
                }
            }

            //if (zoomRouted)
            //	m_NavigationController.NavigateTo<IOsdConferencePresenter>();
            //else if (m_Room.IsInMeeting)
            //	m_NavigationController.NavigateTo<IOsdSourcesPresenter>();
            //else if (m_Room.CalendarControl != null)
            //	m_NavigationController.NavigateTo<IOsdWelcomePresenter>();
        }

        #endregion
    }
}