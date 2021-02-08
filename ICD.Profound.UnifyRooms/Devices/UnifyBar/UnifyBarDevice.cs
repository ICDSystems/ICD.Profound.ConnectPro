using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Network.Attributes.Rpc;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Protocol.Network.Ports.TcpSecure;
using ICD.Connect.Protocol.Network.RemoteProcedure;
using ICD.Connect.Protocol.Ports;

namespace ICD.Profound.UnifyRooms.Devices.UnifyBar
{
	public sealed class UnifyBarDevice : AbstractDevice<UnifyBarDeviceSettings>
	{
		public enum ePage
		{
			Unknown = 0,
			ConferencingApp = 1,
			UnifyApp = 2
		}

		/// <summary>
		/// Raised when the user presses the power button.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnPowerButtonPressed;

		/// <summary>
		/// Raised when the user presses the volume up button.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnVolumeUpButtonPressed;

		/// <summary>
		/// Raised when the user presses the volume down button.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnVolumeDownButtonPressed;

		/// <summary>
		/// Raised when the user presses the volume mute button.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnVolumeMuteButtonPressed;

		/// <summary>
		/// Raised when the user presses the privacy mute button.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnPrivacyMuteButtonPressed;

		/// <summary>
		/// Raised when the user flips to a different page.
		/// </summary>
		public event EventHandler<GenericEventArgs<ePage>> OnPageChanged;

		/// <summary>
		/// Raised when the connected state changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnIsConnectedChanged;

		private readonly INetworkPort m_Client;
		private readonly ClientSerialRpcController m_RpcClient;

		private ePage m_Page;
		private bool m_IsConnected;

		#region Properties

		/// <summary>
		/// Gets the connected state of the client to the unify bar.
		/// </summary>
		public bool IsConnected
		{
			get { return m_IsConnected; }
			private set
			{
				if (value == m_IsConnected)
					return;

				m_IsConnected = value;

				OnIsConnectedChanged.Raise(this, m_IsConnected);
			}
		}

		/// <summary>
		/// Gets the page that the Unify Bar is currently switched to.
		/// </summary>
		public ePage Page
		{
			get { return m_Page; }
			private set
			{
				if (value == m_Page)
					return;

				m_Page = value;

				OnPageChanged.Raise(this, m_Page);
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarDevice()
		{
			m_RpcClient = new ClientSerialRpcController(this);
			Subscribe(m_RpcClient);

			m_Client = new IcdSecureTcpClient
			{
				Address = "localhost",
				Port = 33333
			};

			m_RpcClient.SetPort(m_Client, false);
		}

		#region Private Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnPowerButtonPressed = null;
			OnVolumeUpButtonPressed = null;
			OnVolumeDownButtonPressed = null;
			OnVolumeMuteButtonPressed = null;
			OnPrivacyMuteButtonPressed = null;
			OnPageChanged = null;
			OnIsConnectedChanged = null;

			base.DisposeFinal(disposing);

			Unsubscribe(m_RpcClient);
			m_RpcClient.Dispose();

			m_Client.Dispose();
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Client != null && m_Client.IsOnline;
		}

		#endregion

		#region Rpc Controller Callbacks

		/// <summary>
		/// Subscribe to the RPC controller.
		/// </summary>
		/// <param name="rpcController"></param>
		private void Subscribe(ClientSerialRpcController rpcController)
		{
			rpcController.OnConnectedStateChanged += RpcControllerOnConnectedStateChanged;
			rpcController.OnIsOnlineStateChanged += RpcControllerOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the RPC controller.
		/// </summary>
		/// <param name="rpcController"></param>
		private void Unsubscribe(ClientSerialRpcController rpcController)
		{
			rpcController.OnConnectedStateChanged -= RpcControllerOnConnectedStateChanged;
			rpcController.OnIsOnlineStateChanged -= RpcControllerOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when the online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RpcControllerOnIsOnlineStateChanged(object sender, BoolEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Called when the connected state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RpcControllerOnConnectedStateChanged(object sender, BoolEventArgs args)
		{
			IsConnected = m_RpcClient.IsConnected;

			UpdateCachedOnlineStatus();

			if (args.Data)
				m_RpcClient.CallMethod("GetPage");
			else
				Page = ePage.Unknown;
		}

		#endregion

		#region RPC Methods

		/// <summary>
		/// Sends the powered state to the server.
		/// </summary>
		/// <param name="isPowered"></param>
		public void SetIsPoweredFeedback(bool? isPowered)
		{
			m_RpcClient.CallMethod("SetIsPoweredFeedback", isPowered);
		}

		/// <summary>
		/// Sends the muted state to the server.
		/// </summary>
		/// <param name="isMuted"></param>
		public void SetIsMutedFeedback(bool? isMuted)
		{
			m_RpcClient.CallMethod("SetIsMutedFeedback", isMuted);
		}

		/// <summary>
		/// Sends the privacy muted state to the server.
		/// </summary>
		/// <param name="isPrivacyMuted"></param>
		public void SetIsPrivacyMutedFeedback(bool? isPrivacyMuted)
		{
			m_RpcClient.CallMethod("SetIsPrivacyMutedFeedback", isPrivacyMuted);
		}

		/// <summary>
		/// Sends the lights state to the server.
		/// </summary>
		/// <param name="lights"></param>
		public void SetLightsFeedback(bool? lights)
		{
			m_RpcClient.CallMethod("SetLightsFeedback", lights);
		}

		/// <summary>
		/// Sends the volume ramp enabled state to the server.
		/// </summary>
		/// <param name="volumeEnabled"></param>
		public void SetVolumeRampEnabled(bool volumeEnabled)
		{
			m_RpcClient.CallMethod("SetVolumeRampEnabled", volumeEnabled);
		}

		#endregion

		#region RPC Handlers

		/// <summary>
		/// Called by the server to raise the power button event.
		/// </summary>
		/// <param name="pressed"></param>
		[Rpc("SetPowerButtonPressed")]
		private void HandlePowerButton(bool pressed)
		{
			OnPowerButtonPressed.Raise(this, pressed);
		}

		/// <summary>
		/// Called by the server to raise the volume up button event.
		/// </summary>
		/// <param name="pressed"></param>
		[Rpc("SetVolumeUpButtonPressed")]
		private void HandleVolumeUpButton(bool pressed)
		{
			OnVolumeUpButtonPressed.Raise(this, pressed);
		}

		/// <summary>
		/// Called by the server to raise the volume down button event.
		/// </summary>
		/// <param name="pressed"></param>
		[Rpc("SetVolumeDownButtonPressed")]
		private void HandleVolumeDownButton(bool pressed)
		{
			OnVolumeDownButtonPressed.Raise(this, pressed);
		}

		/// <summary>
		/// Called by the server to raise the volume mute button event.
		/// </summary>
		/// <param name="pressed"></param>
		[Rpc("SetMuteButtonPressed")]
		private void HandleVolumeMuteButton(bool pressed)
		{
			OnVolumeMuteButtonPressed.Raise(this, pressed);
		}

		/// <summary>
		/// Called by the server to raise the privacy mute button event.
		/// </summary>
		/// <param name="pressed"></param>
		[Rpc("SetPrivacyMuteButtonPressed")]
		private void HandlePrivacyMuteButton(bool pressed)
		{
			OnPrivacyMuteButtonPressed.Raise(this, pressed);
		}

		/// <summary>
		/// Called by the server to update the current visible page.
		/// </summary>
		/// <param name="page"></param>
		[Rpc("SetPageChanged")]
		private void HandlePageChanged(int page)
		{
			Page = (ePage)page;
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			m_RpcClient.Stop();

			base.ClearSettingsFinal();
		}

		/// <summary>
		/// Override to add actions on StartSettings
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		protected override void StartSettingsFinal()
		{
			base.StartSettingsFinal();

			m_RpcClient.Start();
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

			addRow("Is Connected", IsConnected);
			addRow("Page", Page);
		}

		#endregion
	}
}
