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
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(IVolumePresenter))]
	public sealed class VolumePresenter : AbstractUiPresenter<IVolumeView>, IVolumePresenter
	{
		private const ushort HIDE_TIME = 20 * 1000;

		/// <summary>
		/// Raised when volume control availability changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnControlAvailableChanged;

		/// <summary>
		/// Raised when volume control mute state changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnControlIsMutedChanged;

		private readonly SafeTimer m_VisibilityTimer;
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly VolumeRepeater m_VolumeRepeater;
		private readonly VolumePointHelper m_VolumePointHelper;

		private bool m_VolumeControlAvailable;
		private bool m_VolumeControlIsMuted;

		private IConferenceManager m_SubscribedConferenceManager;

		#region Properties

		/// <summary>
		/// Gets the volume device control.
		/// </summary>
		[CanBeNull]
		public IVolumeDeviceControl VolumeControl { get { return m_VolumePointHelper.VolumeControl; } }

		/// <summary>
		/// Gets the volume control availability.
		/// </summary>
		private bool VolumeControlAvailable
		{
			get { return m_VolumeControlAvailable; }
			set
			{
				if (value == m_VolumeControlAvailable)
					return;

				m_VolumeControlAvailable = value;

				OnControlAvailableChanged.Raise(this, new BoolEventArgs(m_VolumeControlAvailable));

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the volume control mute state.
		/// </summary>
		private bool VolumeControlIsMuted
		{
			get { return m_VolumeControlIsMuted; }
			set
			{
				if (value == m_VolumeControlIsMuted)
					return;
				
				m_VolumeControlIsMuted = value;

				OnControlIsMutedChanged.Raise(this, new BoolEventArgs(m_VolumeControlIsMuted));

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VolumePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_VisibilityTimer = SafeTimer.Stopped(() => ShowView(false));
			m_VolumeRepeater = new VolumeRepeater();
			m_VolumePointHelper = new VolumePointHelper();

			Subscribe(m_VolumePointHelper);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnControlAvailableChanged = null;
			OnControlIsMutedChanged = null;

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

		private void UpdateVolumePoint()
		{
			m_VolumePointHelper.VolumePoint = Room == null ? null : Room.GetContextualVolumePoints().FirstOrDefault();
		}

		#region Methods

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVolumeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool muted = m_VolumePointHelper.IsMuted;
				float volume = m_VolumePointHelper.GetVolumePercent();
				bool controlAvailable = m_VolumePointHelper.ControlAvailable;

				view.SetMuted(muted);
				view.SetVolumePercentage(volume);
				view.SetControlsEnabled(controlAvailable);
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
			StopVisibilityTimer();

			m_VolumeRepeater.VolumeUpHold(m_VolumePointHelper.VolumePoint);
		}

		/// <summary>
		/// Begins ramping the device volume down.
		/// </summary>
		public void VolumeDown()
		{
			if (!m_VolumePointHelper.SupportedVolumeFeatures.HasFlag(eVolumeFeatures.VolumeAssignment))
				return;

			ShowView(true);
			StopVisibilityTimer();

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
		/// Stops the visibility timer.
		/// </summary>
		public void StopVisibilityTimer()
		{
			m_VisibilityTimer.Stop();
		}

		/// <summary>
		/// Resets the visibility timer.
		/// </summary>
		public void ResetVisibilityTimer()
		{
			m_VisibilityTimer.Reset(HIDE_TIME);
		}

		#endregion

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

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

			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnConferenceParticipantAddedOrRemoved -= ConferenceManagerOnConferenceParticipantAddedOrRemoved;
			m_SubscribedConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;

			m_SubscribedConferenceManager = null;
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

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
			view.OnMuteButtonPressed += ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased += ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed += ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed += ViewOnVolumeDownButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVolumeView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
			view.OnMuteButtonPressed -= ViewOnMuteButtonPressed;
			view.OnVolumeButtonReleased -= ViewOnVolumeButtonReleased;
			view.OnVolumeUpButtonPressed -= ViewOnVolumeUpButtonPressed;
			view.OnVolumeDownButtonPressed -= ViewOnVolumeDownButtonPressed;
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

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
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
		}

		#endregion

		#region Volume Point Helper Callbacks

		/// <summary>
		/// Subscribe to the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Subscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlAvailableChanged += ControlOnControlAvailableChanged;
			volumePointHelper.OnVolumeControlIsMutedChanged += ControlOnIsMutedChanged;
			volumePointHelper.OnVolumeControlVolumeChanged += ControlOnVolumeChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Unsubscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlAvailableChanged -= ControlOnControlAvailableChanged;
			volumePointHelper.OnVolumeControlIsMutedChanged -= ControlOnIsMutedChanged;
			volumePointHelper.OnVolumeControlVolumeChanged -= ControlOnVolumeChanged;
		}

		#endregion

		#region Volume Control Callbacks

		/// <summary>
		/// Called when the volume control availability changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ControlOnControlAvailableChanged(object sender, BoolEventArgs args)
		{
			VolumeControlAvailable = VolumeControl != null && VolumeControl.ControlAvailable;
		}

		/// <summary>
		/// Called when the control volume changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ControlOnVolumeChanged(object sender, EventArgs args)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the control mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ControlOnIsMutedChanged(object sender, BoolEventArgs args)
		{
			VolumeControlIsMuted = VolumeControl != null && VolumeControl.IsMuted;
		}

		#endregion
	}
}
