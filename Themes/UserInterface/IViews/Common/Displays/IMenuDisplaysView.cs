using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays
{
	public interface IMenuDisplaysView : IView
	{
		#region Display 1

		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnDisplay1ButtonPressed;

		/// <summary>
		/// Raised when the user presses the speaker button.
		/// </summary>
		event EventHandler OnDisplay1SpeakerButtonPressed;

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		void SetDisplay1Color(eDisplayColor color);

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		void SetDisplay1Icon(string icon);

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplay1Line1Text(string text);

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplay1Line2Text(string text);

		/// <summary>
		/// Sets the visibility of the speaker button.
		/// </summary>
		/// <param name="visible"></param>
		void ShowDisplay1SpeakerButton(bool visible);

		/// <summary>
		/// Sets the activity state of the speaker button.
		/// </summary>
		/// <param name="active"></param>
		void SetDisplay1SpeakerButtonActive(bool active);

		/// <summary>
		/// Sets the text for the source label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplay1SourceText(string text);

		#endregion

		#region Display 2

		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnDisplay2ButtonPressed;

		/// <summary>
		/// Raised when the user presses the speaker button.
		/// </summary>
		event EventHandler OnDisplay2SpeakerButtonPressed;

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		void SetDisplay2Color(eDisplayColor color);

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		void SetDisplay2Icon(string icon);

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplay2Line1Text(string text);

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplay2Line2Text(string text);

		/// <summary>
		/// Sets the visibility of the speaker button.
		/// </summary>
		/// <param name="visible"></param>
		void ShowDisplay2SpeakerButton(bool visible);

		/// <summary>
		/// Sets the activity state of the speaker button.
		/// </summary>
		/// <param name="active"></param>
		void SetDisplay2SpeakerButtonActive(bool active);

		/// <summary>
		/// Sets the text for the source label.
		/// </summary>
		/// <param name="text"></param>
		void SetDisplay2SourceText(string text);

		#endregion
	}
}
