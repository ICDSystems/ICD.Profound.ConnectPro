using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups
{
	public interface IPopupPresenter<TView> : IUiPresenter<TView>
		where TView : IPopupView
	{
	}
}
