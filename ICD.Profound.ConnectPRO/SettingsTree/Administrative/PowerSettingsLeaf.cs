using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Partitioning.Commercial;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Administrative
{
	public sealed class PowerSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Raised when the display power state changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnDisplayPowerChanged;

		private readonly IcdHashSet<IPowerDeviceControl> m_DisplayPowerControls;
		private bool m_DisplayPower;

		#region Properties

		/// <summary>
		/// Gets the wake schedule.
		/// </summary>
		public WakeSchedule WakeSchedule { get { return Room.WakeSchedule; } }

		/// <summary>
		/// Gets the display power state.
		/// </summary>
		public bool DisplayPower
		{
			get { return m_DisplayPower; }
			private set
			{
				if (value == m_DisplayPower)
					return;

				m_DisplayPower = value;

				OnDisplayPowerChanged.Raise(this, new BoolEventArgs(m_DisplayPower));
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public PowerSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
			m_DisplayPowerControls = new IcdHashSet<IPowerDeviceControl>();

			Name = "Power";
			Icon = SettingsTreeIcons.ICON_POWER;

			RebuildPowerControls();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDisplayPowerChanged = null;

			base.Dispose();

			SetPowerControls(Enumerable.Empty<IPowerDeviceControl>());
		}

		/// <summary>
		/// Toggles the current display power state.
		/// </summary>
		public void ToggleDisplayPower()
		{
			if (DisplayPower)
				m_DisplayPowerControls.ForEach(p => p.PowerOff());
			else
				m_DisplayPowerControls.ForEach(p => p.PowerOn());
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the collection of power controls to match the room destinations.
		/// </summary>
		private void RebuildPowerControls()
		{
			IEnumerable<IPowerDeviceControl> powerControls =
				Room.Routing
				    .Destinations
				    .GetVideoDestinations()
				    .SelectMany(d => d.GetDevices())
				    .SelectMany(d => d.Controls.GetControls<IPowerDeviceControl>());

			SetPowerControls(powerControls);
		}

		/// <summary>
		/// Sets the collection of power controls.
		/// </summary>
		/// <param name="powerControls"></param>
		private void SetPowerControls(IEnumerable<IPowerDeviceControl> powerControls)
		{
			if (powerControls == null)
				throw new ArgumentNullException("powerControls");

			m_DisplayPowerControls.ForEach(Unsubscribe);

			m_DisplayPowerControls.Clear();
			m_DisplayPowerControls.AddRange(powerControls);

			m_DisplayPowerControls.ForEach(Subscribe);

			UpdatePowerState();
		}

		/// <summary>
		/// Updates the current power state to match the state of the power controls.
		/// </summary>
		private void UpdatePowerState()
		{
			DisplayPower =
				m_DisplayPowerControls.Select(c =>
				                              {
					                              switch (c.PowerState)
					                              {
						                              case ePowerState.PowerOn:
						                              case ePowerState.Warming:
							                              return true;

													  case ePowerState.Unknown:
													  case ePowerState.PowerOff:
						                              case ePowerState.Cooling:
							                              return false;

						                              default:
							                              throw new ArgumentOutOfRangeException();
					                              }
				                              })
				                      .Unanimous(false);
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			room.Originators.OnChildrenChanged += OriginatorsOnChildrenChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			room.Originators.OnChildrenChanged -= OriginatorsOnChildrenChanged;
		}

		/// <summary>
		/// Called when the room originators change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OriginatorsOnChildrenChanged(object sender, EventArgs args)
		{
			RebuildPowerControls();
		}

		#endregion

		#region Power Control Callbacks

		/// <summary>
		/// Susbcribe to the power control events.
		/// </summary>
		/// <param name="powerControl"></param>
		private void Subscribe(IPowerDeviceControl powerControl)
		{
			powerControl.OnPowerStateChanged += PowerControlOnPowerStateChanged;
		}

		/// <summary>
		/// Unsubscribe from power control events.
		/// </summary>
		/// <param name="powerControl"></param>
		private void Unsubscribe(IPowerDeviceControl powerControl)
		{
			powerControl.OnPowerStateChanged -= PowerControlOnPowerStateChanged;
		}

		/// <summary>
		/// Called when a power control state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void PowerControlOnPowerStateChanged(object sender, PowerDeviceControlPowerStateApiEventArgs eventArgs)
		{
			UpdatePowerState();
		}

		#endregion
	}
}