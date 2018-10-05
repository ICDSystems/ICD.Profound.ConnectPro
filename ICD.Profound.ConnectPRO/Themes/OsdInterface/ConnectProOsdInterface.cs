using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Sources;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.VisibilityTree;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Welcome;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Views;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class ConnectProOsdInterface : IUserInterface
	{
		private readonly IPanelDevice m_Panel;
		private readonly IOsdNavigationController m_NavigationController;

		private IVisibilityNode m_DefaultNotification;
		private IVisibilityNode m_MainPageVisibility;
		private IVisibilityNode m_NotificationVisibility;

		private IConnectProRoom m_Room;

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
		public ConnectProOsdInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			m_Panel = panel;
			UpdatePanelOnlineJoin();

			IOsdViewFactory viewFactory = new ConnectProOsdViewFactory(panel, theme);
			m_NavigationController = new ConnectProOsdNavigationController(viewFactory, theme);

			BuildVisibilityTree();
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
			// Show "hello" when no notifications are visible
			m_DefaultNotification = new DefaultVisibilityNode(m_NavigationController.LazyLoadPresenter<IHelloPresenter>());
			m_DefaultNotification.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdIncomingCallPresenter>());

			// Only one notification can show at a time, and hide "hello"
			m_NotificationVisibility = new SingleVisibilityNode();
			m_NotificationVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IHelloPresenter>());
			m_NotificationVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdIncomingCallPresenter>());
			
			// these presenters are initially visible
			m_NavigationController.NavigateTo<IHelloPresenter>();
			m_NavigationController.NavigateTo<IOsdHeaderPresenter>();
			
			UpdateVisibility();
		}

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
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs eventArgs)
		{
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			m_NavigationController.LazyLoadPresenter<IOsdWelcomePresenter>().ShowView(m_Room != null && !m_Room.IsInMeeting && m_Room.CalendarControl != null);
			m_NavigationController.LazyLoadPresenter<IOsdSourcesPresenter>().ShowView(m_Room != null && m_Room.IsInMeeting);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			SetRoom(null);

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

			ServiceProvider.GetService<ILoggerService>()
			               .AddEntry(eSeverity.Informational, "{0} setting room to {1}", this, room);

			Unsubscribe(m_Room);
			m_Room = room;
			Subscribe(m_Room);

			m_NavigationController.SetRoom(room);

			UpdatePanelOnlineJoin();
			UpdateVisibility();
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
	}
}
