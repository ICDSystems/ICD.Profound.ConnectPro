using System;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.CUE
{
	public interface ISettingsCueBackgroundView : ITouchDisplayView
	{
		/// <summary>
		/// Raised when the user presses the toggle button.
		/// </summary>
		event EventHandler OnTogglePressed;

		/// <summary>
		/// Raised when the user presses the static button.
		/// </summary>
		event EventHandler OnStaticButtonPressed;

		/// <summary>
		/// Raised when the user presses the seasonal button.
		/// </summary>
		event EventHandler OnSeasonalButtonPressed;

		/// <summary>
		/// Sets the selection state of the static button.
		/// </summary>
		/// <param name="selected"></param>
		void SetStaticButtonSelected(bool selected);

		/// <summary>
		/// Sets the selection state of the seasonal button.
		/// </summary>
		/// <param name="selected"></param>
		void SetSeasonalButtonSelection(bool selected);

		/// <summary>
		/// Sets the toggle state of the toggle button.
		/// </summary>
		/// <param name="selected"></param>
		void SetToggleSelected(bool selected);
	}
}
