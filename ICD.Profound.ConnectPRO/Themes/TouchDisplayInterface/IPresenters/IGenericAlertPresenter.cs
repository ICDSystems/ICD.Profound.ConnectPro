using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters
{
	public interface IGenericAlertPresenter : IUiPresenter<IGenericAlertView>
	{
		/// <summary>
		/// Set the message of the presenter and show it.
		/// </summary>
		/// <param name="message">Message to display</param>
		void Show(string message);

		/// <summary>
		/// Set the message of the presenter and show it for the given time.
		/// </summary>
		/// <param name="message">Message to display</param>
		/// <param name="time">Time in milliseconds to show the popup</param>
		void Show(string message, long time);
	}
}