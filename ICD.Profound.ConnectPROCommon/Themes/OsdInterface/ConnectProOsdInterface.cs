using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Devices;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Bodies;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications;
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
// ReSharper disable NotAccessedField.Local
		private readonly OsdVisibilityTree m_VisibilityTree;
// ReSharper restore NotAccessedField.Local

		private IConnectProRoom m_Room;
		private readonly SafeTimer m_FaceTransitionTimer;
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
			m_FaceTransitionTimer = SafeTimer.Stopped(() => UpdateTouchFree(null));
			UpdatePanelOfflineJoin();

			IOsdViewFactory viewFactory = new ConnectProOsdViewFactory(panel, theme);
			m_NavigationController = new ConnectProOsdNavigationController(viewFactory, theme);

			m_VisibilityTree = new OsdVisibilityTree(m_NavigationController);
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
			UpdateBodyVisibility();
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

		#region Private Methods

		/// <summary>
		/// Shows the current relevant body subpage based on routing and room features.
		/// </summary>
		private void UpdateBodyVisibility()
		{
			if (m_Room == null)
				return;

			IConferenceDeviceControl activeConferenceControl =
				m_Room.Routing
				      .State
				      .GetFakeActiveVideoSources()
				      .SelectMany(kvp => kvp.Value)
				      .Distinct()
				      .SelectMany(s => Room.Core
				                           .Originators
				                           .GetChild<IDevice>(s.Device)
				                           .Controls
				                           .GetControls<IConferenceDeviceControl>())
				      .FirstOrDefault();

			if (activeConferenceControl != null)
				m_NavigationController.NavigateTo<IOsdConferenceBodyPresenter>().ActiveConferenceControl = activeConferenceControl;
			else if (m_Room.IsInMeeting)
				m_NavigationController.NavigateTo<IOsdSourcesBodyPresenter>();
			else if (m_Room.GetCalendarControls().Any())
				m_NavigationController.NavigateTo<IOsdScheduleBodyPresenter>();
			else
			{
				m_VisibilityTree.BodyVisibility.Hide();
				m_NavigationController.NavigateTo<IOsdHelloFooterNotificationPresenter>();
			}

			UpdateTouchFree(null);
		}

		/// <summary>
		/// Updates the Touch-Free feedback for the header, footer notifications, etc
		/// </summary>
		private void UpdateTouchFree(bool? isRunningChanged)
		{
			if (m_Room == null)
				return;

			IOsdHeaderPresenter header = m_NavigationController.LazyLoadPresenter<IOsdHeaderPresenter>();
			IOsdHelloFooterNotificationPresenter footer =
				m_NavigationController.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>();

			bool hasTouchFree = m_Room.TouchFree != null && m_Room.TouchFree.Enabled;

			if (m_Room.MeetingStartTimer.IsRunning && hasTouchFree)
			{
				// Periodically switch between sources and schedule
				if ((m_Room.MeetingStartTimer.Milliseconds / 5000) % 2 == 0)
				{
					if (m_Room.GetCalendarControls().Any())
						m_NavigationController.NavigateTo<IOsdScheduleBodyPresenter>();
					else
					{
						m_VisibilityTree.BodyVisibility.Hide();
						m_NavigationController.NavigateTo<IOsdHelloFooterNotificationPresenter>();
					}
				}
				else
					m_NavigationController.NavigateTo<IOsdSourcesBodyPresenter>();

				// Face starts excited then goes to smiling
				header.FaceImage = m_Room.MeetingStartTimer.Milliseconds >= 1000
					                   ? eTouchFreeFace.Smiling
					                   : eTouchFreeFace.Excited;

				// Update message
				if (m_NavigationController.LazyLoadPresenter<IOsdSourcesBodyPresenter>().IsViewVisible)
					footer.PushMessage("TouchFree", "Don't forget about your other devices");
				else
					footer.PushMessage("TouchFree", "Are you here for this meeting?");
			}
			else if (!m_Room.IsInMeeting && hasTouchFree)
			{
				footer.PushMessage("TouchFree", "Come on in");

				// Periodically whistle
				Random seededRandom = new Random(IcdEnvironment.GetUtcTime().Second / 3); // Whistle for a few seconds at a time
				header.FaceImage =
					seededRandom.Next(0, 2) == 0 // 1 in 3 chance of whistling
						? eTouchFreeFace.Whistling
						: eTouchFreeFace.Neutral;

				m_FaceTransitionTimer.Reset(3 * 1000); // Update in 3 seconds
			}
			else
			{
				footer.ClearMessages("TouchFree");
				header.FaceImage = eTouchFreeFace.None;
			}

			// Was TouchFree cancelled?
			if (hasTouchFree &&
				isRunningChanged.HasValue &&
				!isRunningChanged.Value &&
				!m_Room.MeetingStartTimer.IsElapsed)
			{
				footer.PushMessage("TouchFree", "Touch Free Instant Meeting canceled");
				header.FaceImage = eTouchFreeFace.Crying;

				m_FaceTransitionTimer.Reset(3 * 1000); // Update in 3 seconds
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
			room.Routing.State.OnSourceRoutedChanged += RoutingStateOnSourceRoutedChanged;

			room.MeetingStartTimer.OnMillisecondsChanged += MeetingStartTimerOnMillisecondsChanged;
			room.MeetingStartTimer.OnIsRunningChanged += MeetingStartTimerOnIsRunningChanged;
			room.MeetingStartTimer.OnElapsed += MeetingStartTimerOnElapsed;
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

			room.MeetingStartTimer.OnMillisecondsChanged -= MeetingStartTimerOnMillisecondsChanged;
			room.MeetingStartTimer.OnIsRunningChanged -= MeetingStartTimerOnIsRunningChanged;
			room.MeetingStartTimer.OnElapsed -= MeetingStartTimerOnElapsed;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs eventArgs)
		{
			UpdateBodyVisibility();
		}

		private void RoutingStateOnSourceRoutedChanged(object sender, EventArgs e)
		{
			UpdateBodyVisibility();
		}

		private void MeetingStartTimerOnIsRunningChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateTouchFree(boolEventArgs.Data);
		}

		private void MeetingStartTimerOnMillisecondsChanged(object sender, EventArgs eventArgs)
		{
			UpdateTouchFree(null);
		}

		private void MeetingStartTimerOnElapsed(object sender, EventArgs eventArgs)
		{
			UpdateTouchFree(null);
		}

		#endregion
	}
}
