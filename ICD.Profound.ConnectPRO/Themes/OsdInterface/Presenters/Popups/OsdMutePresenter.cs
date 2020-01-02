using ICD.Common.Utils.EventArguments;
using ICD.Connect.Audio.Utils;
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
		private readonly VolumePointHelper m_VolumePointHelper;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdMutePresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme)
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

			m_VolumePointHelper.VolumePoint = room == null ? null : room.GetVolumePoint();
		}

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
