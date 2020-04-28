using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings
{
	public interface ISettingsBaseView : IPopupView
	{
		/// <summary>
		/// Raised when the user presses one of the settings list items.
		/// </summary>
		event EventHandler<UShortEventArgs> OnPrimaryListItemPressed;

		/// <summary>
		/// Raised when the user presses one of the settings list items.
		/// </summary>
		event EventHandler<UShortEventArgs> OnSecondaryListItemPressed;

		/// <summary>
		/// Sets the labels and icons for the buttons in the list.
		/// </summary>
		/// <param name="labelsAndIcons"></param>
		void SetPrimaryButtonLabels(IEnumerable<KeyValuePair<string, string>> labelsAndIcons);

		/// <summary>
		/// Sets the visibility of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetPrimaryButtonVisible(ushort index, bool visible);

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetPrimaryButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the visibility of the second list of buttons.
		/// </summary>
		/// <param name="visible"></param>
		void SetSecondaryButtonsVisibility(bool visible);

		/// <summary>
		/// Sets the labels and icons for the buttons in the list.
		/// </summary>
		/// <param name="labelsAndIcons"></param>
		void SetSecondaryButtonLabels(IEnumerable<KeyValuePair<string, string>> labelsAndIcons);

		/// <summary>
		/// Sets the visibility of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetSecondaryButtonVisible(ushort index, bool visible);

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetSecondaryButtonSelected(ushort index, bool selected);
	}
}
