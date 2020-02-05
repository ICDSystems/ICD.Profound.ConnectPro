using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.Repeaters;
using ICD.Connect.Audio.Utils;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Header;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters
{
	[PresenterBinding(typeof(IVolumePresenter))]
	public sealed class VolumePresenter : AbstractTouchDisplayPresenter<IVolumeView>, IVolumePresenter
	{
		private const ushort HIDE_TIME = 5 * 1000;

		private readonly SafeTimer m_VisibilityTimer;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly VolumeRepeater m_VolumeRepeater;
		private readonly VolumePointHelper m_VolumePointHelper;
		private readonly HeaderButtonModel m_HeaderVolumeButton;

		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Gets the volume device control.
		/// </summary>
		[CanBeNull]
		public IVolumeDeviceControl VolumeControl { get { return m_VolumePointHelper.VolumeControl; } }

		public VolumePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_VolumePointHelper = new VolumePointHelper();
			m_VolumeRepeater = new VolumeRepeater();

			m_HeaderVolumeButton = new HeaderButtonModel(0, 2, ToggleVolumeVisibility)
			{
				Mode = eHeaderButtonMode.Green,
				Icon = TouchCueIcons.GetIcon(eTouchCueIcon.VolumeUp, eTouchCueColor.White),
				LabelText = "Volume"
			};

			m_VisibilityTimer = SafeTimer.Stopped(() => ShowView(false));

			Subscribe(m_VolumePointHelper);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			m_VisibilityTimer.Dispose();

			Unsubscribe(m_VolumePointHelper);
			m_VolumePointHelper.Dispose();
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			UpdateVolumePoint();
		}

		#region Methods

		protected override void Refresh(IVolumeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool supportsMute = m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Mute);
				bool muted = m_VolumePointHelper.IsMuted;
				float volume = m_VolumePointHelper.GetVolumePercent();
				//bool controlAvailable = m_VolumePointHelper.ControlAvailable;

				view.SetMuteButtonVisible(supportsMute);
				view.SetMuted(muted);
				view.SetVolumePercentage(volume);
				//view.SetControlsEnabled(controlAvailable);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Begins ramping the device volume up.
		/// </summary>
		public void VolumeUp()
		{
			if (!m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
				return;

			ShowView(true);
			ResetVisibilityTimer();

			m_VolumeRepeater.VolumeUpHold(m_VolumePointHelper.VolumePoint);
		}

		/// <summary>
		/// Begins ramping the device volume up.
		/// </summary>
		public void VolumeSet(float percentage)
		{
			if (VolumeControl == null)
				return;

			ShowView(true);
			ResetVisibilityTimer();

			m_VolumePointHelper.SetIsMuted(false);
			m_VolumePointHelper.SetVolumePercent(percentage);
		}
		
		/// <summary>
		/// Begins ramping the device volume down.
		/// </summary>
		public void VolumeDown()
		{
			if (!m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
				return;

			ShowView(true);
			ResetVisibilityTimer();

			m_VolumeRepeater.VolumeDownHold(m_VolumePointHelper.VolumePoint);
		}
		
		/// <summary>
		/// Stops ramping the device volume.
		/// </summary>
		public void Release()
		{
			ResetVisibilityTimer();
			m_VolumeRepeater.Release();
		}

		/// <summary>
		/// Toggles the mute state of the device.
		/// </summary>
		public void ToggleMute()
		{
			if (!m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.Mute))
				return;

			ShowView(true);
			ResetVisibilityTimer();

			m_VolumePointHelper.ToggleIsMuted();
		}

		/// <summary>
		/// Resets the visibility timer.
		/// </summary>
		public void ResetVisibilityTimer()
		{
			m_VisibilityTimer.Reset(HIDE_TIME);
		}

		#endregion

		#region Private Methods

		private void UpdateVolumePoint()
		{
			m_VolumePointHelper.VolumePoint = Room == null ? null : Room.GetContextualVolumePoints().FirstOrDefault();
		}

		private void ToggleVolumeVisibility()
		{
			ShowView(!IsViewVisible);
		}

		#endregion

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;
			
			m_SubscribedConferenceManager.OnConferenceParticipantAddedOrRemoved += ConferenceManagerOnConferenceParticipantAddedOrRemoved;
			m_SubscribedConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
		}

		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;

			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnConferenceParticipantAddedOrRemoved -= ConferenceManagerOnConferenceParticipantAddedOrRemoved;
			m_SubscribedConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;

			m_SubscribedConferenceManager = null;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			IHeaderPresenter header = Navigation.LazyLoadPresenter<IHeaderPresenter>();
			if (e.Data)
				header.AddLeftButton(m_HeaderVolumeButton);
			else
			{
				header.RemoveLeftButton(m_HeaderVolumeButton);
				ShowView(false);
			}

			header.Refresh();
		}

		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs e)
		{
			UpdateVolumePoint();
		}

		private void ConferenceManagerOnConferenceParticipantAddedOrRemoved(object sender, EventArgs e)
		{
			UpdateVolumePoint();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVolumeView view)
		{
			base.Subscribe(view);
			
			view.OnMuteButtonPressed += ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased += ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed += ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed += ViewOnVolumeDownButtonPressed;
			view.OnVolumeGaugePressed += ViewOnVolumeGaugePressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVolumeView view)
		{
			base.Unsubscribe(view);
			
			view.OnMuteButtonPressed -= ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased -= ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed -= ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed -= ViewOnVolumeDownButtonPressed;
			view.OnVolumeGaugePressed -= ViewOnVolumeGaugePressed;
		}

		/// <summary>
		/// Called when the user presses the volume down button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeDownButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumeDown();
		}

		/// <summary>
		/// Called when the user presses the volume up button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeUpButtonPressed(object sender, EventArgs eventArgs)
		{
			VolumeUp();
		}

		/// <summary>
		/// Called when the user releases a volume button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnVolumeButtonReleased(object sender, EventArgs eventArgs)
		{
			Release();
		}

		/// <summary>
		/// Called when the user presses the mute button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnMuteButtonPressed(object sender, EventArgs eventArgs)
		{
			ToggleMute();
		}

		private void ViewOnVolumeGaugePressed(object sender, UShortEventArgs e)
		{
			
			float percentage = e.Data / (float)ushort.MaxValue;
			VolumeSet(percentage);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			ResetVisibilityTimer();
			
			m_HeaderVolumeButton.Selected = args.Data;
		}

		#endregion

		#region VolumePointHelper Callbacks
		
		/// <summary>
		/// Subscribe to the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Subscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlAvailableChanged += ControlOnControlAvailableChanged;
			volumePointHelper.OnVolumeControlIsMutedChanged += DeviceOnMuteStateChanged;
			volumePointHelper.OnVolumeControlVolumeChanged += DeviceOnVolumeChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Unsubscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlAvailableChanged -= ControlOnControlAvailableChanged;
			volumePointHelper.OnVolumeControlIsMutedChanged -= DeviceOnMuteStateChanged;
			volumePointHelper.OnVolumeControlVolumeChanged -= DeviceOnVolumeChanged;
		}

		private void ControlOnControlAvailableChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the control volume changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DeviceOnVolumeChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the control mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void DeviceOnMuteStateChanged(object sender, BoolEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}