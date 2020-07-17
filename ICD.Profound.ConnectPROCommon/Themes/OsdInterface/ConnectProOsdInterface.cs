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
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.HeaderNotifications;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Headers;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.VisibilityTree;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class ConnectProOsdInterface : AbstractUserInterface
	{
		private readonly IPanelDevice m_Panel;
		private readonly IOsdNavigationController m_NavigationController;

		private IVisibilityNode m_DefaultNotification;
		private IVisibilityNode m_MainPageVisibility;
		private IVisibilityNode m_NotificationVisibility;
		private IVisibilityNode m_BannerVisibility;

		private IConnectProRoom m_Room;
		private bool m_UserInterfaceReady;

		#region Properties

		public IPanelDevice Panel { get { return m_Panel; } }

		public override IRoom Room { get { return m_Room; } }

		public override object Target { get { return m_Panel; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProOsdInterface(IPanelDevice panel, IConnectProTheme theme)
		{
			m_Panel = panel;
			UpdatePanelOfflineJoin();

			IOsdViewFactory viewFactory = new ConnectProOsdViewFactory(panel, theme);
			m_NavigationController = new ConnectProOsdNavigationController(viewFactory, theme);

			BuildVisibilityTree();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			SetRoom(null);

			m_NavigationController.Dispose();
		}

		/// <summary>
		/// Updates the "offline" visual state of the panel
		/// </summary>
		private void UpdatePanelOfflineJoin()
		{
			m_Panel.SendInputDigital(CommonJoins.DIGITAL_OFFLINE_JOIN, m_Room == null || !m_UserInterfaceReady);
		}

		/// <summary>
		/// Builds the rules for view visibility, e.g. prevent certain items from being visible at the same time.
		/// </summary>
		private void BuildVisibilityTree()
		{
			// Banner notifications at the top of the CUE
			m_BannerVisibility = new SingleVisibilityNode();
			m_BannerVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdTouchFreeHeaderNotificationPresenter>());

			// Main presenters occupying the middle portion of the CUE
			m_MainPageVisibility = new SingleVisibilityNode();
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdScheduleBodyPresenter>());
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdSourcesBodyPresenter>());
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdConferenceBodyPresenter>());

			// Notifications at the bottom of the CUE
			m_DefaultNotification = new NotificationVisibilityNode(m_NavigationController.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>());
			m_DefaultNotification.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdIncomingCallFooterNotificationPresenter>());
			m_DefaultNotification.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdMuteFooterNotificationPresenter>());

			// These presenters are initially visible
			m_NavigationController.NavigateTo<IOsdHelloFooterNotificationPresenter>();
			m_NavigationController.NavigateTo<IOsdHeaderPresenter>();

			// Always visible
			m_NavigationController.LazyLoadPresenter<IOsdBackgroundPresenter>();
			
			UpdateVisibility();
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
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
			m_UserInterfaceReady = true;
			UpdatePanelOfflineJoin();
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
			room.Routing.State.OnSourceRoutedChanged += RoutingStateOnSourceRoutedChanged;
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
			bool zoomRouted = false;
			foreach (var source in activeSources)
			{
				IOriginator child;
				if (Room.Core.Originators.TryGetChild(source.Device, out child) && child is ZoomRoom)
				{
					zoomRouted = true;
					var control = (child as IDevice).Controls.GetControl<IConferenceDeviceControl>();
					m_NavigationController.LazyLoadPresenter<IOsdConferenceBodyPresenter>().ActiveConferenceControl = control;
					break;
				}
			}

			if (zoomRouted)
				m_NavigationController.NavigateTo<IOsdConferenceBodyPresenter>();
			else if (m_Room.IsInMeeting)
				m_NavigationController.NavigateTo<IOsdSourcesBodyPresenter>();
			else if (m_Room.GetCalendarControls().Any())
				m_NavigationController.NavigateTo<IOsdScheduleBodyPresenter>();
			else
			{
				m_MainPageVisibility.Hide();

				m_NavigationController.NavigateTo<IOsdHelloFooterNotificationPresenter>();
				m_NavigationController.NavigateTo<IOsdHeaderPresenter>();
			}
		}

		#endregion
	}
}
