using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Headers;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Headers
{
	[PresenterBinding(typeof(IOsdHeaderPresenter))]
	public sealed class OsdHeaderPresenter : AbstractOsdPresenter<IOsdHeaderView>, IOsdHeaderPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_RefreshTimer;

		private eTouchFreeFace m_TouchFreeImage;

		public eTouchFreeFace FaceImage
		{
			get { return m_TouchFreeImage; }
			set
			{
				if (value == m_TouchFreeImage)
					return;

				m_TouchFreeImage = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdHeaderPresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_TouchFreeImage = new eTouchFreeFace();

			// Refresh every second to update the time
			m_RefreshTimer = new SafeTimer(RefreshTime, 1000, 1000);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_RefreshTimer.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IOsdHeaderView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string roomName = Room == null ? string.Empty : Room.Name;

				view.SetRoomName(roomName);
				view.SetTouchFreeFaceImage(FaceImage);

				//Shows/Hides critical devices' banner.
				bool criticalDevices = Room != null && Room.GetOfflineCriticalDevicesRecursive().Any();
				view.SetCriticalDevicesBannerVisibility(criticalDevices);

				RefreshTime();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Updates the time label on the header.
		/// </summary>
		private void RefreshTime()
		{
			IOsdHeaderView view = GetView();
			if (view == null)
				return;

			if (!m_RefreshSection.TryEnter())
				return;

			try
			{
				view.SetTimeLabel(Theme.DateFormatting.ShortTime);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}
	}
}
