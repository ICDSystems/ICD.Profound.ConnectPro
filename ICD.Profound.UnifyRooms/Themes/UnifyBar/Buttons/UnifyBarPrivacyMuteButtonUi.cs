using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Profound.UnifyRooms.Devices.UnifyBar;

namespace ICD.Profound.UnifyRooms.Themes.UnifyBar.Buttons
{
	public sealed class UnifyBarPrivacyMuteButtonUi : AbstractUnifyBarButtonUi
	{
		[CanBeNull]
		private IConferenceManager m_ConferenceManager;

		/// <summary>
		/// Gets/sets the conference manager.
		/// </summary>
		[CanBeNull]
		private IConferenceManager ConferenceManager
		{
			get { return m_ConferenceManager; }
			set
			{
				if (value == m_ConferenceManager)
					return;

				Unsubscribe(m_ConferenceManager);
				m_ConferenceManager = value;
				Subscribe(m_ConferenceManager);

				UpdatePrivacyMuteState();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnifyBarPrivacyMuteButtonUi()
		{
			Icon = eMainButtonIcon.PrivacyMute;
			Type = eMainButtonType.Mute;
			Label = "PRIVACY";
		}

		#region Private Methods

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(ICommercialRoom room)
		{
			base.SetRoom(room);

			ConferenceManager = room == null ? null : room.ConferenceManager;

			UpdatePrivacyMuteState();
			UpdateVisibility();
		}

		/// <summary>
		/// Override to implement the button press behaviour.
		/// </summary>
		/// <param name="pressed"></param>
		protected override void HandleButtonPress(bool pressed)
		{
			if (!pressed)
				return;
			
			if (m_ConferenceManager != null)
				m_ConferenceManager.TogglePrivacyMute();
		}

		/// <summary>
		/// Pushes the privacy mute state to the device.
		/// </summary>
		private void UpdatePrivacyMuteState()
		{
			Enabled = m_ConferenceManager != null;
			Selected = m_ConferenceManager != null && m_ConferenceManager.PrivacyMuted;
		}

		/// <summary>
		/// Updates the visibility of the button.
		/// </summary>
		private void UpdateVisibility()
		{
			Visible = m_ConferenceManager != null && m_ConferenceManager.CanPrivacyMute();
		}

		#endregion

		#region Conference Manager Callbacks

		/// <summary>
		/// Subscribe to the conference manager events.
		/// </summary>
		/// <param name="conferenceManager"></param>
		private void Subscribe(IConferenceManager conferenceManager)
		{
			if (conferenceManager == null)
				return;

			conferenceManager.OnPrivacyMuteStatusChange += ConferenceManagerOnPrivacyMuteStatusChange;
			conferenceManager.Dialers.OnConferenceAdded += DialersOnConferenceAdded;
			conferenceManager.Dialers.OnConferenceRemoved += DialersOnConferenceRemoved;
			conferenceManager.VolumePoints.OnVolumePointsChanged += VolumePointsOnVolumePointsChanged;
		}

		/// <summary>
		/// Unsubscribe from the conference manager events.
		/// </summary>
		/// <param name="conferenceManager"></param>
		private void Unsubscribe(IConferenceManager conferenceManager)
		{
			if (conferenceManager == null)
				return;

			conferenceManager.OnPrivacyMuteStatusChange -= ConferenceManagerOnPrivacyMuteStatusChange;
			conferenceManager.Dialers.OnConferenceAdded -= DialersOnConferenceAdded;
			conferenceManager.Dialers.OnConferenceRemoved -= DialersOnConferenceRemoved;
			conferenceManager.VolumePoints.OnVolumePointsChanged -= VolumePointsOnVolumePointsChanged;
		}

		/// <summary>
		/// Called when the 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ConferenceManagerOnPrivacyMuteStatusChange(object sender, BoolEventArgs boolEventArgs)
		{
			UpdatePrivacyMuteState();
		}

		/// <summary>
		/// Called when a conference is added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DialersOnConferenceAdded(object sender, ConferenceEventArgs e)
		{
			UpdateVisibility();
		}

		/// <summary>
		/// Called when a conference is removed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DialersOnConferenceRemoved(object sender, ConferenceEventArgs e)
		{
			UpdateVisibility();
		}

		/// <summary>
		/// Called when a volume point is added/removed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VolumePointsOnVolumePointsChanged(object sender, EventArgs e)
		{
			UpdateVisibility();
		}

		#endregion
	}
}
