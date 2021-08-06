using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Network.Attributes.Rpc;
using ICD.Connect.Protocol.Network.Ports;
using ICD.Connect.Protocol.Network.Ports.TcpSecure;
using ICD.Connect.Protocol.Network.RemoteProcedure;
using ICD.Profound.UnifyRooms.MoreControls;

namespace ICD.Profound.UnifyRooms.Devices.UnifyBar
{
	public sealed class UnifyBarDevice : AbstractDevice<UnifyBarDeviceSettings>
	{
		/// <summary>
		/// Raised when the user presses a main button.
		/// </summary>
		public event EventHandler<MainButtonPressedEventArgs> OnMainButtonPressed;

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
		private readonly UnifyBarMainButton[] m_MainButtons;

		private ePage m_Page;
		private bool m_IsConnected;
		private int m_MainButtonCount;
		private MoreControlsPanelConfiguration m_XPanelConfiguration;

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

				UpdateCachedOnlineStatus();

				if (m_IsConnected)
					Initialize();

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
			m_MainButtons = Enumerable.Range(0, 6).Select(i => new UnifyBarMainButton(this, i)).ToArray();
			m_XPanelConfiguration = new MoreControlsPanelConfiguration();

			m_RpcClient = new ClientSerialRpcController(this);
			Subscribe(m_RpcClient);

			m_Client = new IcdSecureTcpClient
			{
				Address = "localhost",
				Port = 33333,
				Name = "UnifyBarClient"
			};

			m_RpcClient.SetPort(m_Client, false);

			foreach (UnifyBarMainButton button in m_MainButtons)
				Subscribe(button);
		}

		#region Methods

		/// <summary>
		/// Gets the main buttons.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<UnifyBarMainButton> GetMainButtons()
		{
			return m_MainButtons.ToArray();
		}

		/// <summary>
		/// Sets the number of main buttons.
		/// </summary>
		/// <param name="count"></param>
		public void SetMainButtonCount(int count)
		{
			m_MainButtonCount = count;
			SetMainButtonCountRpc(m_MainButtonCount);
		}

		/// <summary>
		/// Configures the custom programmed XPanel.
		/// </summary>
		/// <param name="configuration"></param>
		public void ConfigurePanel([NotNull] MoreControlsPanelConfiguration configuration)
		{
			m_XPanelConfiguration = configuration;
			ConfigurePanelRpc(m_XPanelConfiguration);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			OnMainButtonPressed = null;
			OnPageChanged = null;
			OnIsConnectedChanged = null;

			foreach (UnifyBarMainButton button in m_MainButtons)
				button.Dispose();

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

		/// <summary>
		/// Sets up the initial device state.
		/// </summary>
		private void Initialize()
		{
			SetMainButtonCountRpc(m_MainButtonCount);

			foreach (UnifyBarMainButton button in m_MainButtons)
			{
				SetMainButtonSelectedRpc(button.Index, button.Selected);
				SetMainButtonEnabledRpc(button.Index, button.Enabled);
				SetMainButtonLabelRpc(button.Index, button.Label);
				SetMainButtonTypeRpc(button.Index, button.Type);
				SetMainButtonIconRpc(button.Index, button.Icon);
			}

			if (m_Page != ePage.Unknown)
				SetPageRpc(m_Page);

			ConfigurePanelRpc(m_XPanelConfiguration);
		}

		#endregion

		#region Button Callbacks

		/// <summary>
		/// Subscribe to the button events.
		/// </summary>
		/// <param name="button"></param>
		private void Subscribe(UnifyBarMainButton button)
		{
			button.OnSelectedChanged += ButtonOnSelectedChanged;
			button.OnEnabledStateChanged += ButtonOnEnabledStateChanged;
			button.OnLabelChanged += ButtonOnLabelChanged;
			button.OnIconChanged += ButtonOnIconChanged;
			button.OnTypeChanged += ButtonOnTypeChanged;
		}

		private void ButtonOnTypeChanged(object sender, GenericEventArgs<eMainButtonType> e)
		{
			UnifyBarMainButton button = (UnifyBarMainButton)sender;
			SetMainButtonTypeRpc(button.Index, button.Type);
		}

		private void ButtonOnIconChanged(object sender, GenericEventArgs<eMainButtonIcon> e)
		{
			UnifyBarMainButton button = (UnifyBarMainButton)sender;
			SetMainButtonIconRpc(button.Index, button.Icon);
		}

		private void ButtonOnLabelChanged(object sender, StringEventArgs e)
		{
			UnifyBarMainButton button = (UnifyBarMainButton)sender;
			SetMainButtonLabelRpc(button.Index, button.Label);
		}

		private void ButtonOnEnabledStateChanged(object sender, BoolEventArgs e)
		{
			UnifyBarMainButton button = (UnifyBarMainButton)sender;
			SetMainButtonEnabledRpc(button.Index, button.Enabled);
		}

		private void ButtonOnSelectedChanged(object sender, BoolEventArgs e)
		{
			UnifyBarMainButton button = (UnifyBarMainButton)sender;
			SetMainButtonSelectedRpc(button.Index, button.Selected);
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
		}

		#endregion

		#region RPC Methods

		private void SetMainButtonCountRpc(int count)
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("SetMainButtonCount", count);
		}

		/// <summary>
		/// Sets the selected state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		private void SetMainButtonSelectedRpc(int index, bool selected)
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("SetMainButtonSelected", index, selected);
		}

		/// <summary>
		/// Sets the enabled state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		private void SetMainButtonEnabledRpc(int index, bool enabled)
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("SetMainButtonEnabled", index, enabled);
		}

		/// <summary>
		/// Sets the label of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		private void SetMainButtonLabelRpc(int index, string label)
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("SetMainButtonLabel", index, label);
		}

		/// <summary>
		/// Sets the type of button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="type"></param>
		private void SetMainButtonTypeRpc(int index, eMainButtonType type)
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("SetMainButtonType", index, (int)type);
		}

		/// <summary>
		/// Sets the icon of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		private void SetMainButtonIconRpc(int index, eMainButtonIcon icon)
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("SetMainButtonIcon", index, (int)icon);
		}

		/// <summary>
		/// Sets the current page.
		/// </summary>
		/// <param name="page"></param>
		private void SetPageRpc(ePage page)
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("SetPage", (int)page);
		}

		/// <summary>
		/// Asks the server for the current page.
		/// </summary>
		private void UpdatePageRpc()
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("GetPage");
		}

		private void ConfigurePanelRpc([NotNull] MoreControlsPanelConfiguration configuration)
		{
			if (m_RpcClient.IsConnected)
				m_RpcClient.CallMethod("ConfigureXPanel",
				                       configuration.Enabled,
				                       configuration.Path,
				                       configuration.Hostname,
				                       configuration.Port,
				                       configuration.Ipid);
		}

		#endregion

		#region RPC Handlers

		/// <summary>
		/// Called by the server to raise the power button event.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="pressed"></param>
		[Rpc("SetMainButtonPressed")]
		private void HandleMainButtonPressed(int index, bool pressed)
		{
			OnMainButtonPressed.Raise(this, new MainButtonPressedEventArgs(index, pressed));
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

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			yield return m_Client;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}
