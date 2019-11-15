using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions
{
	public interface IFloatingActionView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the option button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the selected state of the option.
		/// </summary>
		/// <param name="active"></param>
		void SetActive(bool active);

		/// <summary>
		/// Sets the enabled state of the option.
		/// </summary>
		/// <param name="enabled"></param>
		void SetEnabled(bool enabled);
	}
}
