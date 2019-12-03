using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras
{
	public interface ICameraButtonsView : IPopupView
	{
		/// <summary>
		/// Raised when one of the camera configuration buttons are pressed (Control, Active, or Layout).
		/// </summary>
		event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Sets the selected state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the visible state of a camera configuration button.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetButtonVisible(ushort index, bool visible);

		/// <summary>
		/// Sets the labels for the buttons.
		/// </summary>
		/// <param name="buttons"></param>
		void SetButtons(IEnumerable<string> buttons);
	}
}
