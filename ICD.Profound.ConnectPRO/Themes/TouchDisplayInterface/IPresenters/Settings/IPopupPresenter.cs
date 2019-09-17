using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Settings
{
	public interface IPopupPresenter<TView> : ITouchDisplayPresenter<TView>
		where TView : IPopupView
	{
		/// <summary>
		/// Closes the popup.
		/// </summary>
		void Close();
	}
}
