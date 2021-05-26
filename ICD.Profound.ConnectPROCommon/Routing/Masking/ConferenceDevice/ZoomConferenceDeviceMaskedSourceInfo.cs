using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Controls.Conferencing;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Routing.Masking.ConferenceDevice
{
	public sealed class ZoomConferenceDeviceMaskedSourceInfo : AbstractConferenceDeviceMaskedSourceInfo
	{
		private ZoomRoomConferenceControl m_Control;

		[CanBeNull]
		private ZoomRoomConferenceControl Control
		{
			get { return m_Control; }
			set
			{
				if (value == m_Control)
					return;

				Unsubscribe(m_Control);
				m_Control = value;
				Subscribe(m_Control);

				UpdateMask();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ZoomConferenceDeviceMaskedSourceInfo(IConnectProRoom room)
			: base(room)
		{
		}

		protected override void UpdateControl()
		{
			base.UpdateControl();

			ZoomRoom device = Source == null ? null : Room.Core.Originators.GetChild(Source.Device) as ZoomRoom;

			Control = device == null ? null : device.Controls.GetControl<ZoomRoomConferenceControl>();
		}

		/// <summary>
		/// Returns true if the conference source should currently be masked.
		/// </summary>
		/// <returns></returns>
		protected override bool ShouldBeMasked()
		{
			bool connectingOrConnected =
				Control != null &&
				Control.GetConferences()
				       .Any(c => c.Status == eConferenceStatus.Connecting ||
				                 c.Status == eConferenceStatus.Connected);

			return !connectingOrConnected && base.ShouldBeMasked();
		}

		#region Conference Control Callbacks

		private void Subscribe(ZoomRoomConferenceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);
		}

		private void Unsubscribe(ZoomRoomConferenceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded -= ControlOnConferenceAdded;
			control.OnConferenceRemoved -= ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Unsubscribe(conference);
		}

		private void ControlOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data);

			UpdateMask();
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data);

			UpdateMask();
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs e)
		{
			UpdateMask();
		}

		#endregion
	}
}
