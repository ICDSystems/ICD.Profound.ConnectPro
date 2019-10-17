using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.EventArguments;
using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Settings;

namespace ICD.Profound.ConnectPRO.Devices
{
	public sealed class ConnectProEventServerDevice : AbstractDevice<ConnectProEventServerDeviceSettings>
	{
		private const string DELIMITER = "\xFF";

		private readonly IcdTcpServer m_Server;
		private readonly Dictionary<string, string> m_Cache;
		private readonly SafeCriticalSection m_CacheSection;

		/// <summary>
		/// Gets/sets the port for the server.
		/// </summary>
		[PublicAPI]
		public ushort Port
		{
			get { return m_Server.Port; }
			set
			{
				if (value == m_Server.Port)
					return;

				m_Server.Port = value;

				if (m_Server.Enabled)
					m_Server.Restart();

				UpdateCachedOnlineStatus();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProEventServerDevice()
		{
			m_Cache = new Dictionary<string, string>();
			m_CacheSection = new SafeCriticalSection();

			m_Server = new IcdTcpServer
			{
				Name = GetType().Name,
				MaxNumberOfClients = IcdTcpServer.MAX_NUMBER_OF_CLIENTS_SUPPORTED
			};

			Subscribe(m_Server);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			Unsubscribe(m_Server);

			base.DisposeFinal(disposing);

			m_Server.Dispose();
		}

		#region Methods

		/// <summary>
		/// Clears all of the cached values.
		/// </summary>
		public void ClearCache()
		{
			m_CacheSection.Execute(() => m_Cache.Clear());
		}

		/// <summary>
		/// Sets the message for the given arbitrary key.
		/// If the message has changed it will be sent to all connected clients.
		/// Cached messages will be sent to new clients.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		public void SendMessage(string key, string message)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			m_CacheSection.Enter();

			try
			{
				string oldvalue;
				if (m_Cache.TryGetValue(key, out oldvalue) && message == oldvalue)
					return;

				m_Cache[key] = message;
			}
			finally
			{
				m_CacheSection.Leave();
			}

			Log(eSeverity.Informational, message);

			SendMessage(message);
		}

		/// <summary>
		/// Sends the given message to all connected clients.
		/// </summary>
		/// <param name="message"></param>
		public void SendMessage(string message)
		{
			m_Server.Send(message + DELIMITER);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Server != null && m_Server.Listening;
		}

		/// <summary>
		/// Sends the cached messages to the given client.
		/// </summary>
		/// <param name="clientId"></param>
		private void SendCache(uint clientId)
		{
			string data;

			m_CacheSection.Enter();

			try
			{
				if (m_Cache.Count == 0)
					return;

				data = string.Join(DELIMITER, m_Cache.Values.ToArray(m_Cache.Count)) + DELIMITER;
			}
			finally
			{
				m_CacheSection.Leave();
			}

			m_Server.Send(clientId, data);
		}

		#endregion

		#region Server Callbacks

		/// <summary>
		/// Subscribe to the server events.
		/// </summary>
		/// <param name="server"></param>
		private void Subscribe(IcdTcpServer server)
		{
			server.OnSocketStateChange += ServerOnSocketStateChange;
		}

		/// <summary>
		/// Unsubscribe from the server events.
		/// </summary>
		/// <param name="server"></param>
		private void Unsubscribe(IcdTcpServer server)
		{
			server.OnSocketStateChange -= ServerOnSocketStateChange;
		}

		/// <summary>
		/// Called when a client connection state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ServerOnSocketStateChange(object sender, SocketStateEventArgs eventArgs)
		{
			if (eventArgs.SocketState == SocketStateEventArgs.eSocketStatus.SocketStatusConnected)
				SendCache(eventArgs.ClientId);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			ClearCache();

			base.ClearSettingsFinal();

			Port = 0;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ConnectProEventServerDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Port = Port;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ConnectProEventServerDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			Port = settings.Port;

			ClearCache();
			m_Server.Start();

			UpdateCachedOnlineStatus();
		}

		#endregion
	}
}