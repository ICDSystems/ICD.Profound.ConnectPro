using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
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
		private DateTime m_TouchFreeCancellationTime;

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
			m_FaceTransitionTimer = SafeTimer.Stopped(UpdateTouchFree);
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

			UpdateTouchFree();
		}

		#endregion

		#region TouchFree

		/// <summary>
		/// Updates the Touch-Free feedback for the header, footer notifications, etc
		/// </summary>
		private void UpdateTouchFree()
		{
			if (m_Room == null)
				return;

			if (m_Room.TouchFreeEnabled && !m_Room.IsInMeeting)
			{
				// TouchFree is counting down
				if (m_Room.MeetingStartTimer.IsRunning && !m_Room.MeetingStartTimer.IsElapsed)
					UpdateTouchFreeCountdown();
				// TouchFree countdown isn't running
				else
					UpdateTouchFreeIdle();
			}
			else
			{
				UpdateTouchFreeDisabled();
			}
		}

		/// <summary>
		/// Clears the TouchFree face animation and footer notification.
		/// </summary>
		private void UpdateTouchFreeDisabled()
		{
			IOsdHeaderPresenter header = m_NavigationController.LazyLoadPresenter<IOsdHeaderPresenter>();
			IOsdHelloFooterNotificationPresenter footer =
				m_NavigationController.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>();

			footer.ClearMessages("TouchFree");
			header.FaceImage = eTouchFreeFace.None;
		}

		/// <summary>
		/// Updates the idle state of the footer label and makes the face periodically whistle.
		/// </summary>
		private void UpdateTouchFreeIdle()
		{
			IOsdHeaderPresenter header = m_NavigationController.LazyLoadPresenter<IOsdHeaderPresenter>();
			IOsdHelloFooterNotificationPresenter footer =
				m_NavigationController.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>();

			if (IcdEnvironment.GetUtcTime() - m_TouchFreeCancellationTime > TimeSpan.FromSeconds(5))
			{
				footer.PushMessage("TouchFree", "Come on in");

				// Periodically whistle
				int seed = (int)((IcdEnvironment.GetUtcTime().GetTotalSeconds() / 3) % int.MaxValue); // Whistle for a few seconds at a time
				Random seededRandom = new Random(seed);

				header.FaceImage =
					seededRandom.Next(0, 9) == 0 // 1 in 10 chance of whistling
						? new[] {eTouchFreeFace.Whistle, eTouchFreeFace.Sleepy}.Random(seededRandom)
						: eTouchFreeFace.Daydream;
			}
			else
			{
				footer.PushMessage("TouchFree", "Touch Free Instant Meeting canceled");
				header.FaceImage = eTouchFreeFace.Sad;
			}

			m_FaceTransitionTimer.Reset(1000); // Update in a second
		}

		/// <summary>
		/// Handles the header face image, footer notification and body cycling
		/// while the TouchFree countdown is running.
		/// </summary>
		private void UpdateTouchFreeCountdown()
		{
			IOsdHeaderPresenter header = m_NavigationController.LazyLoadPresenter<IOsdHeaderPresenter>();
			IOsdHelloFooterNotificationPresenter footer =
				m_NavigationController.LazyLoadPresenter<IOsdHelloFooterNotificationPresenter>();

			long halfDuration = m_Room.MeetingStartTimer.Length / 2;

			// Show schedule presenter for the first half of the time, then switch to sources presenter. 
			if (m_Room.MeetingStartTimer.Milliseconds <= halfDuration)
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
			{
				m_NavigationController.NavigateTo<IOsdSourcesBodyPresenter>();
			}

			// Face starts excited then goes to smiling
			header.FaceImage = m_Room.MeetingStartTimer.Milliseconds >= 1000
				                   ? eTouchFreeFace.Happy
				                   : eTouchFreeFace.Surprised;

			// Update message
			if (m_NavigationController.LazyLoadPresenter<IOsdSourcesBodyPresenter>().IsViewVisible)
				footer.PushMessage("TouchFree", "Don't forget about your other devices");
			else
				footer.PushMessage("TouchFree", "Are you here for this meeting?");
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

			room.OnTouchFreeEnabledChanged += RoomOnTouchFreeEnabledChanged;
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

			room.OnTouchFreeEnabledChanged -= RoomOnTouchFreeEnabledChanged;
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

		private void RoomOnTouchFreeEnabledChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateTouchFree();
		}

		private void MeetingStartTimerOnIsRunningChanged(object sender, BoolEventArgs boolEventArgs)
		{
			bool cancelled = m_Room != null && !m_Room.MeetingStartTimer.IsElapsed && !boolEventArgs.Data;
			m_TouchFreeCancellationTime = cancelled ? IcdEnvironment.GetUtcTime() : DateTime.MinValue;
			
			// Ensure we land on the correct body presenter - calls UpdateTouchFree
			UpdateBodyVisibility();
		}

		private void MeetingStartTimerOnMillisecondsChanged(object sender, EventArgs eventArgs)
		{
			UpdateTouchFree();
		}

		private void MeetingStartTimerOnElapsed(object sender, EventArgs eventArgs)
		{
			UpdateTouchFree();
		}

		#endregion
	}
}
