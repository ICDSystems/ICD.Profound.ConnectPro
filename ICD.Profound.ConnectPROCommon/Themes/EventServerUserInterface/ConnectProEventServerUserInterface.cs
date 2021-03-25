using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Themes.UserInterfaces;
using ICD.Profound.ConnectPROCommon.Devices;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface
{
	public sealed class ConnectProEventServerUserInterface : AbstractUserInterface
	{
		private readonly List<IEventServerKeyHandler> m_Handlers; 
		private readonly ConnectProEventServerDevice m_Device;

		[CanBeNull]
		private IConnectProRoom m_Room;

		#region Properties

		/// <summary>
		/// Gets the room attached to this UI.
		/// </summary>
		[CanBeNull]
		public override IRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets the event server device.
		/// </summary>
		public ConnectProEventServerDevice Device { get { return m_Device; } }

		/// <summary>
		/// Gets the target instance attached to this UI.
		/// </summary>
		public override object Target { get { return Device; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="theme"></param>
		public ConnectProEventServerUserInterface(ConnectProEventServerDevice device, IConnectProTheme theme)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			m_Device = device;

			m_Handlers = new List<IEventServerKeyHandler>
			{
				new ActiveCameraEventServerKeyHandler(theme, device),
				new AudioSourcesEventServerKeyHandler(theme, device),
				new AwakeEventServerKeyHandler(theme, device),
				new CameraPrivacyMuteEventServerKeyHandler(theme, device),
				new IncomingCallEventServerKeyHandler(theme, device),
				new IsInCallEventServerKeyHandler(theme, device),
				new MeetingEventServerKeyHandler(theme, device),
				new PrivacyMuteEventServerKeyHandler(theme, device),
				new RoomCombineEventServerKeyHandler(theme, device),
				new VideoSourcesEventServerKeyHandler(theme, device)
			};
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			SetRoom(null);

			foreach (IEventServerKeyHandler handler in m_Handlers)
				handler.Dispose();
			m_Handlers.Clear();
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

			m_Room = room;

			foreach (IEventServerKeyHandler handler in m_Handlers)
				handler.SetRoom(m_Room);
		}

		/// <summary>
		/// Tells the UI that it should be considered ready to use.
		/// For example updating the online join on a panel or starting a long-running process that should be delayed.
		/// </summary>
		public override void Activate()
		{
		}

		#endregion
	}
}
