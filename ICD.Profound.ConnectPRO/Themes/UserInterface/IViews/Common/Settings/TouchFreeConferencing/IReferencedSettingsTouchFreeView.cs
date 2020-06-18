using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing
{
	public interface IReferencedSettingsTouchFreeView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		void SetIcon(string icon);

		/// <summary>
		/// Sets the text for the label.
		/// </summary>
		/// <param name="text"></param>
		void SetText(string text);

		/// <summary>
		/// Sets the selected state of the button.
		/// </summary>
		/// <param name="selected"></param>
		void SetSelected(bool selected);
	}
}
