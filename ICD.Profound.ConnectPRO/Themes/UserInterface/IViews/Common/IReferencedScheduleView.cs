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
		void SetLine1Text(string text);

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		void SetLine2Text(string text);

		/// <summary>
		/// Sets the text for the third label.
		/// </summary>
		/// <param name="text"></param>
		void SetLine3Text(string text);

		/// <summary>
		/// Sets the text for the forth label.
		/// </summary>
		/// <param name="text"></param>
		void SetLine4Text(string text);

		/// <summary>
		/// Sets the text for the fifth label.
		/// </summary>
		/// <param name="text"></param>
		void SetLine5Text(string text);
	}
}
