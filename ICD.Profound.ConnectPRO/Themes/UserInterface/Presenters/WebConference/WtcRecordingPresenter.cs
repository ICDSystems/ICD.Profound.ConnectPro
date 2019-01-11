using ICD.Common.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public class WtcRecordingPresenter : AbstractWtcPresenter<IWtcRecordingView>, IWtcRecordingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		public WtcRecordingPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IWtcRecordingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{

			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}
	}
}