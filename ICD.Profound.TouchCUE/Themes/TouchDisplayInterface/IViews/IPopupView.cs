using System;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews
{
	public interface IPopupView : ITouchDisplayView
	{
		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		event EventHandler OnCloseButtonPressed;
	}
}