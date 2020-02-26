using ICD.Connect.Audio.Controls.Mute;
using ICD.Connect.Audio.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters.Popups
{
	[PresenterBinding(typeof(IOsdMutePresenter))]
	public sealed class OsdMutePresenter : AbstractOsdPresenter<IOsdMuteView>, IOsdMutePresenter
	{
		private IVolumeMuteFeedbackDeviceControl m_SubscribedControl;
		private IVolumeMuteFeedbackDeviceControl MuteControl
		{
			get { return m_SubscribedControl; }
			set
			{
				if (m_SubscribedControl == value)
					return;

				if(m_SubscribedControl != null)
					Unsubscribe(m_SubscribedControl);

				m_SubscribedControl = value;

				if (m_SubscribedControl != null)
					Subscribe(m_SubscribedControl);

				UpdateVisibility();
			}
		}

		public OsdMutePresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			if (room == null)
				return;
			
			MuteControl = room.GetVolumeControl() as IVolumeMuteFeedbackDeviceControl;
		}

		private void UpdateVisibility()
		{
			if (MuteControl != null && MuteControl.VolumeIsMuted)
				ShowView(true);
			else
				ShowView(false);
		}

		#region Mute Control Callbacks

		private void Subscribe(IVolumeMuteFeedbackDeviceControl control)
		{
			control.OnMuteStateChanged += ControlOnMuteStateChanged;
		}

		private void Unsubscribe(IVolumeMuteFeedbackDeviceControl control)
		{
			control.OnMuteStateChanged -= ControlOnMuteStateChanged;
		}
		
		private void ControlOnMuteStateChanged(object sender, MuteDeviceMuteStateChangedApiEventArgs eventArgs)
		{
			UpdateVisibility();
		}

		#endregion
	}
}
