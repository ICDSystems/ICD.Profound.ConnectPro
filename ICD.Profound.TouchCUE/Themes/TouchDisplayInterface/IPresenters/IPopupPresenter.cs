using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters
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
