using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.EventArguments;
using ICD.Connect.Protocol.Network.Ports.Tcp;
using ICD.Connect.Protocol.Network.Servers;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Settings;

namespace ICD.Profound.ConnectPROCommon.Devices
{
	/// <summary>
	/// Sends/receives arbitrary messages in the format KEY:VALUE\xFF.
	/// Enables duplex event synchronization with basic consumers (e.g. Simpl Windows, Zoom Room Controls)
	/// </summary>
	public sealed class ConnectProEventServerDevice : AbstractDevice<ConnectProEventServerDeviceSettings>
	{
		public delegate void InputCallback(ConnectProEventServerDevice sender, string message);

		public const ushort DEFAULT_PORT = 8888;

		private const char OUTPUT_DELIMITER = '\xFF';
		private const char KVP_DELIMITER = ':';

		private static readonly char[] s_InputDelimiters =
		{
			OUTPUT_DELIMITER,
			'\r',
			'\n'
		};

		/// <summary>
		/// States from the control system to the connected clients.
		/// </summary>
		private readonly IcdSortedDictionary<string, string> m_OutputCache;

		/// <summary>
		/// States from the connected clients to the control system.
		/// </summary>
		private readonly IcdSortedDictionary<string, string> m_InputCache;

		/// <summary>
		/// Registered callbacks for input key changes.
		/// </summary>
		private readonly Dictionary<string, IcdHashSet<InputCallback>> m_InputCallbacks; 

		private readonly IcdTcpServer m_Server;
		private readonly NetworkServerBufferManager m_ClientBuffers;
		private readonly SafeCriticalSection m_CacheSection;

		#region Properties

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

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProEventServerDevice()
		{
			m_InputCache = new IcdSortedDictionary<string, string>();
			m_OutputCache = new IcdSortedDictionary<string, string>();
			m_InputCallbacks = new Dictionary<string, IcdHashSet<InputCallback>>();
			m_CacheSection = new SafeCriticalSection();

			m_Server = new IcdTcpServer
			{
				Name = GetType().Name,
				MaxNumberOfClients = IcdTcpServer.MAX_NUMBER_OF_CLIENTS_SUPPORTED
			};

			m_ClientBuffers = new NetworkServerBufferManager(() => new MultiDelimiterSerialBuffer(s_InputDelimiters));
			m_ClientBuffers.SetServer(m_Server);

			Subscribe(m_Server);
			Subscribe(m_ClientBuffers);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			m_CacheSection.Execute(() => m_InputCallbacks.Clear());

			Unsubscribe(m_Server);
			Unsubscribe(m_ClientBuffers);

			base.DisposeFinal(disposing);

			m_ClientBuffers.Dispose();
			m_Server.Dispose();
		}

		#region Methods

		/// <summary>
		/// Clears all of the cached values.
		/// </summary>
		public void ClearCache()
		{
			m_CacheSection.Execute(() => m_OutputCache.Clear());
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
				if (m_OutputCache.TryGetValue(key, out oldvalue) && message == oldvalue)
					return;

				m_OutputCache[key] = message;
			}
			finally
			{
				m_CacheSection.Leave();
			}

			string data = FormatMessage(key, message);

			Logger.Log(eSeverity.Informational, "Output - {0}", data.Substring(0, data.Length - 1));
			m_Server.Send(data);
		}

		/// <summary>
		/// Registers a handler for the given input key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="callback"></param>
		public void RegisterInputKeyHandler([NotNull] string key, [NotNull] InputCallback callback)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			if (callback == null)
				throw new ArgumentNullException("callback");

			m_CacheSection.Execute(() => m_InputCallbacks.GetOrAddNew(key).Add(callback));
		}

		/// <summary>
		/// Deregisters a handler for the given input key.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="callback"></param>
		public void DeregisterInputKeyHandler([NotNull] string key, [NotNull] InputCallback callback)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			if (callback == null)
				throw new ArgumentNullException("callback");

			m_CacheSection.Enter();

			try
			{
				IcdHashSet<InputCallback> callbacks;
				if (m_InputCallbacks.TryGetValue(key, out callbacks))
					callbacks.Remove(callback);
			}
			finally
			{
				m_CacheSection.Leave();
			}
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
		private void SendOutputCache(uint clientId)
		{
			string data;

			m_CacheSection.Enter();

			try
			{
				if (m_OutputCache.Count == 0)
					return;

				data = m_OutputCache.Select(kvp => FormatMessage(kvp)).Join();
			}
			finally
			{
				m_CacheSection.Leave();
			}

			m_Server.Send(clientId, data);
		}

		/// <summary>
		/// Formats the kev-value data into a message.
		/// </summary>
		/// <param name="kvp"></param>
		/// <returns></returns>
		private static string FormatMessage(KeyValuePair<string, string> kvp)
		{
			return FormatMessage(kvp.Key, kvp.Value);
		}

		/// <summary>
		/// Formats the kev-value data into a message.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		private static string FormatMessage(string key, string message)
		{
			return string.Format("{0}{1}{2}{3}", key, KVP_DELIMITER, message, OUTPUT_DELIMITER);
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
				SendOutputCache(eventArgs.ClientId);
		}

		#endregion

		#region Buffer Manager Callbacks

		/// <summary>
		/// Subscribe to the buffer manager events.
		/// </summary>
		/// <param name="bufferManager"></param>
		private void Subscribe(NetworkServerBufferManager bufferManager)
		{
			bufferManager.OnClientCompletedSerial += BufferManagerOnClientCompletedSerial;
		}

		/// <summary>
		/// Unsubscribe from the buffer manager events.
		/// </summary>
		/// <param name="bufferManager"></param>
		private void Unsubscribe(NetworkServerBufferManager bufferManager)
		{
			bufferManager.OnClientCompletedSerial -= BufferManagerOnClientCompletedSerial;
		}

		/// <summary>
		/// Called when a complete message is received from a client.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="clientId"></param>
		/// <param name="data"></param>
		private void BufferManagerOnClientCompletedSerial(NetworkServerBufferManager sender, uint clientId, string data)
		{
			const string pattern = @"^(?'key'[^:]*):(?'value'.*)$";

			Match match = Regex.Match(data, pattern);
			if (!match.Success)
			{
				Logger.Log(eSeverity.Warning, "Discarding malformed input - {0}", data);
				return;
			}

			Logger.Log(eSeverity.Informational, "Input - {0}", data);

			string key = match.Groups["key"].Value;
			string value = match.Groups["value"].Value;

			InputCallback[] callbacks;

			m_CacheSection.Enter();

			try
			{
				string oldvalue;
				if (m_InputCache.TryGetValue(key, out oldvalue) && value == oldvalue)
					return;

				m_InputCache[key] = value;

				IcdHashSet<InputCallback> callbacksSet;
				if (m_InputCallbacks.TryGetValue(key, out callbacksSet))
					callbacks = callbacksSet.ToArray();
				else
					return;
			}
			finally
			{
				m_CacheSection.Leave();
			}

			foreach (InputCallback callback in callbacks)
				callback(this, value);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			m_Server.Stop();
			ClearCache();

			base.ClearSettingsFinal();

			Port = DEFAULT_PORT;
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
		}

		/// <summary>
		/// Override to add actions on StartSettings
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		protected override void StartSettingsFinal()
		{
			base.StartSettingsFinal();

			m_Server.Start();
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Port", Port);
		}

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			yield return m_Server;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("PrintInputCache", "Prints the table of messages from the connected clients to the server", () => PrintInputCache());
			yield return new ConsoleCommand("PrintOutputCache", "Prints the table of messages from the server to the connected clients", () => PrintOutputCache());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		/// <summary>
		/// Prints the table of messages from the connected clients to the server.
		/// </summary>
		/// <returns></returns>
		private string PrintInputCache()
		{
			TableBuilder builder = new TableBuilder("Key", "Value");

			m_CacheSection.Enter();

			try
			{
				foreach (KeyValuePair<string, string> kvp in m_InputCache)
					builder.AddRow(kvp.Key, kvp.Value);
			}
			finally
			{
				m_CacheSection.Leave();
			}

			return builder.ToString();
		}

		/// <summary>
		/// Prints the table of messages from the server to the connected clients.
		/// </summary>
		/// <returns></returns>
		private string PrintOutputCache()
		{
			TableBuilder builder = new TableBuilder("Key", "Value");

			m_CacheSection.Enter();

			try
			{
				foreach (KeyValuePair<string, string> kvp in m_OutputCache)
					builder.AddRow(kvp.Key, kvp.Value);
			}
			finally
			{
				m_CacheSection.Leave();
			}

			return builder.ToString();
		}

		#endregion
	}
}
