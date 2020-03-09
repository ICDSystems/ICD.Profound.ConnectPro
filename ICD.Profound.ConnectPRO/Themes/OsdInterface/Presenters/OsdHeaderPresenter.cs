using ICD.Common.Utils;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters
{
	[PresenterBinding(typeof(IOsdHeaderPresenter))]
	public sealed class OsdHeaderPresenter : AbstractOsdPresenter<IOsdHeaderView>, IOsdHeaderPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_RefreshTimer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdHeaderPresenter(IOsdNavigationController nav, IOsdViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

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
