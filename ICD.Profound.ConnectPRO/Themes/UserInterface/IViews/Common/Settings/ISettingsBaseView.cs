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
		/// Sets the selected state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetItemSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the labels for the buttons in the list.
		/// </summary>
		/// <param name="labels"></param>
		void SetButtonLabels(IEnumerable<string> labels);

		/// <summary>
		/// Sets the visibility of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetButtonVisible(ushort index, bool visible);
	}
}
