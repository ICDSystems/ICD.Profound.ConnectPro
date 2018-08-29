using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups
{
	public interface IPopupPresenter<TView> : IPresenter<TView>
		where TView : IPopupView
	{
		/// <summary>
		/// Closes the popup.
		/// </summary>
		void Close();
	}
}
