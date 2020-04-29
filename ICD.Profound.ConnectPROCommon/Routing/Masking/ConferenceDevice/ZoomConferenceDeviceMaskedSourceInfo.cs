using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Routing.Masking.ConferenceDevice
{
	public sealed class ZoomConferenceDeviceMaskedSourceInfo : AbstractConferenceDeviceMaskedSourceInfo
	{
		private ZoomRoomConferenceControl m_Control;
		private ZoomRoomTraditionalConferenceControl m_TraditionalControl;

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

		[CanBeNull]
		private ZoomRoomTraditionalConferenceControl TraditionalControl
		{
			get { return m_TraditionalControl; }
			set
			{
				if (value == m_TraditionalControl)
					return;
				
				Unsubscribe(m_TraditionalControl);
				m_TraditionalControl = value;
				Subscribe(m_TraditionalControl);

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
			TraditionalControl = device == null ? null : device.Controls.GetControl<ZoomRoomTraditionalConferenceControl>();
		}

		/// <summary>
		/// Returns true if the conference source should currently be masked.
		/// </summary>
		/// <returns></returns>
		protected override bool ShouldBeMasked()
		{
			// In a call-out call
			bool connectingOrConnected =
				TraditionalControl != null &&
				TraditionalControl.GetConferences()
				                  .Any(c => c.Status == eConferenceStatus.Connecting ||
				                            c.Status == eConferenceStatus.Connected);

			// In a zoom room
			connectingOrConnected |=
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

		#region Traditional Conference Control Callbacks

		private void Subscribe(ZoomRoomTraditionalConferenceControl traditionalControl)
		{
			if (traditionalControl == null)
				return;

			traditionalControl.OnConferenceAdded += TraditionalControlOnConferenceAdded;
			traditionalControl.OnConferenceRemoved += TraditionalControlOnConferenceRemoved;

			foreach (var conference in traditionalControl.GetConferences())
				Subscribe(conference);
		}

		private void Unsubscribe(ZoomRoomTraditionalConferenceControl traditionalControl)
		{
			if (traditionalControl == null)
				return;

			traditionalControl.OnConferenceAdded -= TraditionalControlOnConferenceAdded;
			traditionalControl.OnConferenceRemoved -= TraditionalControlOnConferenceRemoved;

			foreach (var conference in traditionalControl.GetConferences())
				Unsubscribe(conference);
		}

		private void TraditionalControlOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			Subscribe(e.Data);

			UpdateMask();
		}

		private void TraditionalControlOnConferenceRemoved(object sender, ConferenceEventArgs e)
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
