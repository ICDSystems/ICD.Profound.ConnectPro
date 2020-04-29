using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls;
using ICD.Connect.Misc.Yepkit.Devices.YkupSwitcher;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.YkupSwitcherInterface
{
	public sealed class ConnectProYkupSwitcherInterface : AbstractUserInterface
	{
		public const int VIBE_OUTPUT = 1;
		public const int ZOOM_OUTPUT = 2;

		private bool m_IsDisposed;

		private readonly IConnectProTheme m_Theme;
		private readonly YkupSwitcherDevice m_Switcher;

		[CanBeNull]
		private IConnectProRoom m_Room;

		private VibeBoardAppControl m_SubscribedAppControl;

		#region Properties

		[CanBeNull]
		public override IRoom Room { get { return m_Room; } }

		public YkupSwitcherDevice Switcher { get { return m_Switcher; } }

		public override object Target { get { return Switcher; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="switcher"></param>
		/// <param name="theme"></param>
		public ConnectProYkupSwitcherInterface(YkupSwitcherDevice switcher, IConnectProTheme theme)
		{
			m_Switcher = switcher;
			m_Theme = theme;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_IsDisposed = true;

			SetRoom(null);
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
		/// Sets the room for this interface.
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

			if (!m_IsDisposed)
				UpdateSwitcher();
		}

		/// <summary>
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
			UpdateSwitcher();
		}

		#endregion

		#region Room Callbacks

		private void Subscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
			room.Routing.State.OnSourceRoutedChanged += RoomRoutingStateOnSourceRoutedChanged;

			var vibeBoard = room.Originators.GetInstanceRecursive<VibeBoard>();
			m_SubscribedAppControl = vibeBoard == null ? null : vibeBoard.Controls.GetControl<VibeBoardAppControl>();

			if (m_SubscribedAppControl != null)
				m_SubscribedAppControl.OnAppLaunched += SubscribedAppControlOnAppLaunched;
		}

		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
			room.Routing.State.OnSourceRoutedChanged -= RoomRoutingStateOnSourceRoutedChanged;

			if (m_SubscribedAppControl != null)
				m_SubscribedAppControl.OnAppLaunched -= SubscribedAppControlOnAppLaunched;
			m_SubscribedAppControl = null;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			Switcher.Route(ZOOM_OUTPUT);
		}

		private void RoomRoutingStateOnSourceRoutedChanged(object sender, EventArgs e)
		{
			UpdateSwitcher();
		}

		private void SubscribedAppControlOnAppLaunched(object sender, EventArgs e)
		{
			Switcher.Route(VIBE_OUTPUT);
		}

		/// <summary>
		/// Using USB switcher output 1 (to Vibe) when using a source/app other than zoom.
		/// Using USB switcher output 2 (to Zoom) otherwise.
		/// </summary>
		private void UpdateSwitcher()
		{
			if (m_Room == null)
				return;

			IEnumerable<ISource> active =
				m_Room.Routing
				      .State
				      .GetFakeActiveVideoSources()
				      .SelectMany(kvp => kvp.Value);

			foreach (ISource source in active)
			{
				Switcher.Route(IsZoom(source) ? ZOOM_OUTPUT : VIBE_OUTPUT);
				return;
			}
		}

		/// <summary>
		/// Returns true if the given source represents a Zoom Room device.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		private bool IsZoom(ISource source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (m_Room == null)
				throw new InvalidOperationException("No room");

			return m_Room.Core.Originators.GetChild(source.Device) is ZoomRoom;
		}

		#endregion
	}
}
