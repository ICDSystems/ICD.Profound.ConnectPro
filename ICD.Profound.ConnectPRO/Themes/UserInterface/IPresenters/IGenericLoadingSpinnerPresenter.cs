using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	public interface IGenericLoadingSpinnerPresenter : IUiPresenter<IGenericLoadingSpinnerView>
	{
		/// <summary>
		/// Shows the view with the text parameter displayed
		/// </summary>
		/// <param name="text"></param>
		void ShowView(string text);

		/// <summary>
		/// Shows the view with the text parameter displayed, and times out after timeoutMilliseconds
		/// </summary>
		/// <param name="text"></param>
		/// <param name="timeoutMilliseconds"></param>
		void ShowView(string text, long timeoutMilliseconds);

		/// <summary>
		/// Time out the spinner with a generic message if the task takes too long
		/// </summary>
		void TimeOut();

		/// <summary>
		/// Time out the spinner with a specific message if the task takes too long
		/// </summary>
		/// <param name="text"></param>
		void TimeOut(string text);
	}
}