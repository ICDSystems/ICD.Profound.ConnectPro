using ICD.Connect.Audio.Controls.Volume;
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
		private IVolumeDeviceControl m_SubscribedControl;

		private IVolumeDeviceControl VolumeControl
		{
			get { return m_SubscribedControl; }
			set
			{
				if (m_SubscribedControl == value)
					return;

				Unsubscribe(m_SubscribedControl);
				m_SubscribedControl = value;
				Subscribe(m_SubscribedControl);

				UpdateVisibility();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdMutePresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			VolumeControl = room == null ? null : room.GetVolumeControl();
		}

		private void UpdateVisibility()
		{
			if (VolumeControl != null && VolumeControl.IsMuted)
				ShowView(true);
			else
				ShowView(false);
		}

		#region Volume Control Callbacks

		/// <summary>
		/// Subscribe to the control events.
		/// </summary>
		/// <param name="control"></param>
		private void Subscribe(IVolumeDeviceControl control)
		{
			control.OnIsMutedChanged += ControlOnMuteStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the control events.
		/// </summary>
		/// <param name="control"></param>
		private void Unsubscribe(IVolumeDeviceControl control)
		{
			control.OnIsMutedChanged -= ControlOnMuteStateChanged;
		}
		
		/// <summary>
		/// Called when the mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ControlOnMuteStateChanged(object sender, VolumeControlIsMutedChangedApiEventArgs eventArgs)
		{
			UpdateVisibility();
		}

		#endregion
	}
}
