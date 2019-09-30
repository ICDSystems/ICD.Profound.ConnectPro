using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays
{
	public interface IReferencedDisplayView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnDisplayButtonPressed;

		/// <summary>
		/// Raised when the user presses the speaker button.
		/// </summary>
		event EventHandler OnDisplaySpeakerButtonPressed;

		/// <summary>
		/// Sets the color style of the entire view.
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

		/// <summary>
		/// Warming/cooling bar graph - show/hide and set position and text
		/// </summary>
		/// <param name="visible"></param>
		/// <param name="position"></param>
		/// <param name="text"></param>
		void SetDisplayStatusGauge(bool visible, ushort position, string text);
	}
}
