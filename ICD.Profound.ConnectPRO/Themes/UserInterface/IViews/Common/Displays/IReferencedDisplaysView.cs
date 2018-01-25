using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays
{
	public interface IReferencedDisplaysView : IView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Raised when the user presses the speaker button.
		/// </summary>
		event EventHandler OnSpeakerButtonPressed;

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		void SetColor(eDisplayColor color);

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		void SetIcon(string icon);

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
		/// Sets the visibility of the speaker button.
		/// </summary>
		/// <param name="visible"></param>
		void ShowSpeakerButton(bool visible);
	}
}
