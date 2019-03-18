using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays
{
	public interface IMenuCombinedSimpleModeView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the advanced mode button.
		/// </summary>
		event EventHandler OnAdvancedModeButtonPressed;

		/// <summary>
		/// Raised when the user presses the display button.
		/// </summary>
		event EventHandler OnDisplayButtonPressed;

		/// <summary>
		/// Raised when the user pressed the speaker button.
		/// </summary>
		event EventHandler OnDisplaySpeakerButtonPressed;

		/// <summary>
		/// Sets the enabled state of the Advanced Mode button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetAdvancedModeEnabled(bool enabled);

		/// <summary>
		/// Sets the color for the button.
		/// </summary>
		/// <param name="color"></param>
		void SetDisplayColor(eDisplayColor color);

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		void SetDisplayIcon(string icon);

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplayLine1Text(string text);

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplayLine2Text(string text);

		/// <summary>
		/// Sets the visibility of the speaker button.
		/// </summary>
		/// <param name="visible"></param>
		void ShowDisplaySpeakerButton(bool visible);

		/// <summary>
		/// Sets the activity state of the speaker button.
		/// </summary>
		/// <param name="active"></param>
		void SetDisplaySpeakerButtonActive(bool active);

		/// <summary>
		/// Sets the text for the source label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplaySourceText(string text);
	}
}
