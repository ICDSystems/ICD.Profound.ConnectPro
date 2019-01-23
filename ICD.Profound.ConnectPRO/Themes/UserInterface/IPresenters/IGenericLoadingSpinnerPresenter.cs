using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	public interface IGenericLoadingSpinnerPresenter : IUiPresenter<IGenericLoadingSpinnerView>
	{
		void ShowView(string text);
	}
}