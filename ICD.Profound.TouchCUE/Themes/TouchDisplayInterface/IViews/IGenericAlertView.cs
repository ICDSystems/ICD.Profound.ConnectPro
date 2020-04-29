using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews
{
	public interface IGenericAlertView : ITouchDisplayView
	{
		/// <summary>
		/// Raised when the user presses a button.
		/// </summary>
		event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Sets the text prompt.
		/// </summary>
		/// <param name="text"></param>
		void SetAlertText(string text);

		/// <summary>
		/// Sets the number of buttons and their labels.
		/// </summary>
		/// <param name="buttons"></param>
		void SetButtons(IEnumerable<string> buttons);

		/// <summary>
		/// Sets the enabled state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		void SetButtonEnabled(ushort index, bool enabled);

		/// <summary>
		/// Sets the visibility of button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetButtonVisible(ushort index, bool visible);

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetButtonSelected(ushort index, bool selected);
	}
}