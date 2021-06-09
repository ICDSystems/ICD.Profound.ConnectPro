using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Presentation;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Routing.Masking.ConferenceDevice
{
	public abstract class AbstractConferenceDeviceMaskedSourceInfo : AbstractMaskedSourceInfo
	{
		private IConferenceDeviceControl m_ConferenceControl;
		private IPresentationControl m_PresentationControl;

		#region Properties

		[CanBeNull]
		protected IConferenceDeviceControl ConferenceControl
		{
			get { return m_ConferenceControl; }
			set
			{
				if (value == m_ConferenceControl)
					return;

				Unsubscribe(m_ConferenceControl);
				m_ConferenceControl = value;
				Subscribe(m_ConferenceControl);

				UpdateMask();
			}
		}

		[CanBeNull]
		protected IPresentationControl PresentationControl
		{
			get { return m_PresentationControl; }
			set
			{
				if (value == m_PresentationControl)
					return;

				Unsubscribe(m_PresentationControl);
				m_PresentationControl = value;
				Subscribe(m_PresentationControl);

				UpdateMask();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		protected AbstractConferenceDeviceMaskedSourceInfo(IConnectProRoom room)
			: base(room)
		{
		}

		/// <summary>
		/// Override to perform the unmasking operation.
		/// </summary>
		protected override void PerformUnmask()
		{
			if (Source != null)
				Room.Routing.RouteVtc(Source, this);
		}

		/// <summary>
		/// Updates the mask state to match the current state of the controls.
		/// </summary>
		protected void UpdateMask()
		{
			Mask = ShouldBeMasked();
		}

		/// <summary>
		/// Returns true if the conference source should currently be masked.
		/// </summary>
		/// <returns></returns>
		protected virtual bool ShouldBeMasked()
		{
			// Presenting
			if (PresentationControl != null && PresentationControl.PresentationActive)
				return false;

			// In a call
			if (ConferenceControl != null && ConferenceControl.GetActiveConferences().Any())
				return false;

			return true;
		}

		protected virtual void UpdateControl()
		{
			IDevice device = Source == null ? null : Room.Core.Originators.GetChild<IDevice>(Source.Device);

			ConferenceControl = device == null ? null : device.Controls.GetControl<IConferenceDeviceControl>();
			PresentationControl = device == null ? null : device.Controls.GetControl<IPresentationControl>();

			UpdateMask();
		}

		#region Source Callbacks 

		protected override void Subscribe(ISource source)
		{
			base.Subscribe(source);

			UpdateControl();
		}

		protected override void Unsubscribe(ISource source)
		{
			base.Unsubscribe(source);

			UpdateControl();
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
			UpdateMask();
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
