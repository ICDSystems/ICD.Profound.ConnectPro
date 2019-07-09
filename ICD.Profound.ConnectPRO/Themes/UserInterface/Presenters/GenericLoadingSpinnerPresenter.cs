using ICD.Common.Utils;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	[PresenterBinding(typeof(IGenericLoadingSpinnerPresenter))]
	public sealed class GenericLoadingSpinnerPresenter : AbstractUiPresenter<IGenericLoadingSpinnerView>, IGenericLoadingSpinnerPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private string LoadingText { get; set; }

		public GenericLoadingSpinnerPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IGenericLoadingSpinnerView view)
		{
			m_RefreshSection.Execute(() => view.SetText(LoadingText));
		}

		public void ShowView(string text)
		{
			LoadingText = text;
			ShowView(true);
			RefreshIfVisible();
		}
	}
}