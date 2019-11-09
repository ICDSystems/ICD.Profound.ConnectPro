using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing.Masking.ConferenceDevice
{
	public sealed class ZoomConferenceDeviceMaskedSourceInfo : AbstractConferenceDeviceMaskedSourceInfo
	{
		private ZoomRoomTraditionalConferenceControl m_TraditionalControl;

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

			var device = Source == null ? null : Room.Core.Originators.GetChild(Source.Device) as ZoomRoom;
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
			if (connectingOrConnected)
				return false;

			return base.ShouldBeMasked();
		}

		#region Conference Control Callbacks

		private void Subscribe(ZoomRoomTraditionalConferenceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);
		}

		private void Unsubscribe(ZoomRoomTraditionalConferenceControl control)
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
