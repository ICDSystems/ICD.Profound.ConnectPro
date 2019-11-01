using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom
{
	public interface ISettingsZoomView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the general button.
		/// </summary>
		event EventHandler OnGeneralButtonPressed;

		/// <summary>
		/// Raised when the user presses the advanced button.
		/// </summary>
		event EventHandler OnAdvancedButtonPressed;

		/// <summary>
		/// Sets the selection state of the general button.
		/// </summary>
		/// <param name="selected"></param>
		void SetGeneralButtonSelected(bool selected);

		/// <summary>
		/// Sets the selection state of the advanced button.
		/// </summary>
		/// <param name="selected"></param>
		void SetAdvancedButtonSelection(bool selected);
	}
}
