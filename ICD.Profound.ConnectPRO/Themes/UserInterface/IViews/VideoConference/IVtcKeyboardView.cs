using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcKeyboardView : IVtcBaseKeyboardView
	{
		/// <summary>
		/// Raised when the user presses the space button.
		/// </summary>
		event EventHandler OnSpaceButtonPressed;

		/// <summary>
		/// Raised when the user presses the caps button.
		/// </summary>
		event EventHandler OnCapsButtonPressed;

		/// <summary>
		/// Raised when the user presses the shift button.
		/// </summary>
		event EventHandler OnShiftButtonPressed;

		/// <summary>
		/// Raised when the user presses the exit button.
		/// </summary>
		event EventHandler OnKeypadButtonPressed;

		/// <summary>
		/// Sets the selected state of the caps button.
		/// </summary>
		void SelectCapsButton(bool select);

		/// <summary>
		/// Sets the selected state of the shift button.
		/// </summary>
		void SelectShiftButton(bool select);

		/// <summary>
		/// Sets the shift state of the chars.
		/// </summary>
		/// <param name="shift"></param>
		/// <param name="caps"></param>
		void SetShift(bool shift, bool caps);
	}
}
