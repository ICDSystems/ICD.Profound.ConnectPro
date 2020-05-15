using System;
using System.Linq;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Utils;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Popups;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Popups
{
	[PresenterBinding(typeof(IOsdMutePresenter))]
	public sealed class OsdMutePresenter : AbstractOsdPresenter<IOsdMuteView>, IOsdMutePresenter
	{
		private readonly VolumePointHelper m_VolumePointHelper;

		private IConferenceManager m_SubscribedConferenceManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdMutePresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_VolumePointHelper = new VolumePointHelper();
			Subscribe(m_VolumePointHelper);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_VolumePointHelper);
			m_VolumePointHelper.Dispose();
		}

		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			UpdateVolumePoint();
		}

		private void UpdateVolumePoint()
		{
			m_VolumePointHelper.VolumePoint = Room == null ? null : Room.GetContextualVolumePoints().FirstOrDefault();
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			m_SubscribedConferenceManager = room == null ? null : room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.Dialers.OnConferenceParticipantAddedOrRemoved += ConferenceManagerOnConferenceParticipantAddedOrRemoved;
			m_SubscribedConferenceManager.Dialers.OnInCallChanged += ConferenceManagerOnInCallChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.Dialers.OnConferenceParticipantAddedOrRemoved -= ConferenceManagerOnConferenceParticipantAddedOrRemoved;
			m_SubscribedConferenceManager.Dialers.OnInCallChanged -= ConferenceManagerOnInCallChanged;

			m_SubscribedConferenceManager = null;
		}

		/// <summary>
		/// Called when a participant is added to or removed from an active conference.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ConferenceManagerOnConferenceParticipantAddedOrRemoved(object sender, EventArgs eventArgs)
		{
			UpdateVolumePoint();
		}

		/// <summary>
		/// Called when a call starts/stops.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="inCallEventArgs"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs inCallEventArgs)
		{
			UpdateVolumePoint();
		}

		#endregion

		#region Volume Point Helper Callbacks

		/// <summary>
		/// Subscribe to the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Subscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlIsMutedChanged += VolumePointHelperOnVolumeControlIsMutedChanged;
		}

		/// <summary>
		/// Unsubscribe from the volume point helper events.
		/// </summary>
		/// <param name="volumePointHelper"></param>
		private void Unsubscribe(VolumePointHelper volumePointHelper)
		{
			volumePointHelper.OnVolumeControlIsMutedChanged -= VolumePointHelperOnVolumeControlIsMutedChanged;
		}
		
		/// <summary>
		/// Called when the mute state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void VolumePointHelperOnVolumeControlIsMutedChanged(object sender, BoolEventArgs eventArgs)
		{
			ShowView(m_VolumePointHelper.IsMuted);
		}

		#endregion
	}
}
