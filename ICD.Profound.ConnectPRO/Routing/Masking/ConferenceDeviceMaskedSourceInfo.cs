using System.Linq;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing.Masking
{
	public sealed class ConferenceDeviceMaskedSourceInfo : AbstractMaskedSourceInfo
	{
		private IConferenceDeviceControl m_ConferenceControl;
		private IPresentationControl m_PresentationControl;
		private IConnectProRoom m_Room;

		#region Properties

		public IConnectProRoom Room
		{
			get { return m_Room; }
			private set
			{
				if (m_Room == value)
					return;
				m_Room = value;
				UpdateControl();
			}
		}

		private IConferenceDeviceControl ConferenceControl
		{
			get { return m_ConferenceControl; }
			set
			{
				if (m_ConferenceControl == value)
					return;

				Unsubscribe(m_ConferenceControl);
				m_ConferenceControl = value;
				Subscribe(m_ConferenceControl);
			}
		}

		#endregion

		public ConferenceDeviceMaskedSourceInfo(ISource source, IConnectProRoom room)
		{
			Room = room;
			Source = source;
		}

		protected override void PerformMask()
		{
			Room.Routing.RouteOsd(this);
		}

		protected override void PerformUnmask()
		{
			Room.Routing.RouteVtc(Source, this);
		}

		private void UpdateMask()
		{
			if (m_ConferenceControl == null)
				return;

			Mask = ConferenceControl.GetConferences().All(c => c.Status != eConferenceStatus.Connected);
		}

		private void UpdateControl()
		{
			if (Source == null || Room == null)
			{
				ConferenceControl = null;
				return;
			}

			var device = Room.Core.Originators.GetChild(Source.Device) as IDevice;
			ConferenceControl = device == null ? null : device.Controls.GetControl<IConferenceDeviceControl>();

			Unsubscribe(m_PresentationControl);
			m_PresentationControl = device == null ? null : device.Controls.GetControl<IPresentationControl>();
			Subscribe(m_PresentationControl);

			UpdateMask();
		}

		#region Source Callbacks 

		protected override void Subscribe(ISource source)
		{
			base.Subscribe(source);

			if (source == null)
				return;

			UpdateControl();
		}

		protected override void Unsubscribe(ISource source)
		{
			base.Unsubscribe(source);
			
			if (source == null || Room == null)
				return;

			ConferenceControl = null;
		}

		#endregion

		#region Conference Control Callbacks

		private void Subscribe(IConferenceDeviceControl control)
		{
			if (control == null)
				return;

			control.OnConferenceAdded += ControlOnConferenceAdded;
			control.OnConferenceRemoved += ControlOnConferenceRemoved;

			foreach (var conference in control.GetConferences())
				Subscribe(conference);
		}

		private void Unsubscribe(IConferenceDeviceControl control)
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
		}

		private void ControlOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			Unsubscribe(e.Data);
		}

		#endregion

		#region Presentation Control Callbacks

		private void Subscribe(IPresentationControl control)
		{
			if (control == null)
				return;

			control.OnPresentationActiveChanged += ControlOnPresentationActiveChanged;
		}

		private void Unsubscribe(IPresentationControl control)
		{
			if (control == null)
				return;

			control.OnPresentationActiveChanged -= ControlOnPresentationActiveChanged;
		}

		private void ControlOnPresentationActiveChanged(object sender, PresentationActiveApiEventArgs args)
		{
			if (Room == null || ConferenceControl == null)
				return;

			if (Room.IsCombineRoom() && ConferenceControl.GetActiveConference() != null)
				Room.Routing.RouteVtc(Source, this);
		}

		#endregion

		#region Conference Callbacks

		private void Subscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged += ConferenceOnStatusChanged;
		}

		private void Unsubscribe(IConference conference)
		{
			if (conference == null)
				return;

			conference.OnStatusChanged -= ConferenceOnStatusChanged;
		}

		private void ConferenceOnStatusChanged(object sender, ConferenceStatusEventArgs e)
		{
			UpdateMask();
		}

		#endregion
	}
}
