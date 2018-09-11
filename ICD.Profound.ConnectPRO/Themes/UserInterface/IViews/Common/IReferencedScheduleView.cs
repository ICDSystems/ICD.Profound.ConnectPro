using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common
{
	public interface IReferencedScheduleView : IView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		void SetDayLabel(string text);

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		void SetStartTimeLabel(string text);

		/// <summary>
		/// Sets the text for the third label.
		/// </summary>
		/// <param name="text"></param>
		void SetBodyLabel(string text);

		/// <summary>
		/// Sets the text for the forth label.
		/// </summary>
		/// <param name="text"></param>
		void SetEndTimeLabel(string text);

		/// <summary>
		/// Sets the text for the fifth label.
		/// </summary>
		/// <param name="text"></param>
		void SetPresenterNameLabel(string text);

		/// <summary>
		/// Sets the background button selected state.
		/// </summary>
		/// <param name="selected"></param>
		void SetSelected(bool selected);
	}
}
