using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings
{
	public interface ISettingsBaseView : IPopupView
	{
		/// <summary>
		/// Raised when the user presses one of the settings list items.
		/// </summary>
		event EventHandler<UShortEventArgs> OnListItemPressed;

		/// <summary>
		/// Raised when the user presses the back button.
		/// </summary>
		event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Sets the selected state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetItemSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the labels and icons for the buttons in the list.
		/// </summary>
		/// <param name="labelsAndIcons"></param>
		void SetButtonLabels(IEnumerable<KeyValuePair<string, string>> labelsAndIcons);

		/// <summary>
		/// Sets the visibility of the button at the given index.
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

		/// <summary>
		/// Sets the visibility of the back button.
		/// </summary>
		/// <param name="visible"></param>
		void SetBackButtonVisible(bool visible);

		/// <summary>
		/// Sets the text for the title label.
		/// </summary>
		/// <param name="title"></param>
		void SetTitle(string parent, string leaf);
	}
}
