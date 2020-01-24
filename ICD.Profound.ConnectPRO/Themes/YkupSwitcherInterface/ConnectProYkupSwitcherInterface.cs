using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Misc.Yepkit.Devices.YkupSwitcher;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.YkupSwitcherInterface
{
	public sealed class ConnectProYkupSwitcherInterface : AbstractUserInterface
	{
		private bool m_IsDisposed;

		private readonly ConnectProTheme m_Theme;
		private readonly YkupSwitcherDevice m_Switcher;

		[CanBeNull]
		private IConnectProRoom m_Room;

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
		public ConnectProYkupSwitcherInterface(YkupSwitcherDevice switcher, ConnectProTheme theme)
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

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Subscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.Routing.State.OnDisplaySourceChanged += StateOnDisplaySourceChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.Routing.State.OnDisplaySourceChanged -= StateOnDisplaySourceChanged;
		}

		/// <summary>
		/// Called when a source becomes routed/unrouted to a display.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StateOnDisplaySourceChanged(object sender, EventArgs eventArgs)
		{
			UpdateSwitcher();
		}

		/// <summary>
		/// Using USB switcher output 1 (to Vibe) when not using Zoom.
		/// Using USB switcher output 2 (to Zoom) when using Zoom.
		/// </summary>
		private void UpdateSwitcher()
		{
			if (m_Room == null)
				return;

			bool zoomActive =
				m_Room.Routing
				      .State
				      .GetFakeActiveVideoSources()
				      .SelectMany(kvp => kvp.Value)
				      .Any(IsZoom);

			int output = zoomActive ? 2 : 1;

			Switcher.Route(output);
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
