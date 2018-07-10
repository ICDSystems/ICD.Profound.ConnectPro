using System;
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
		/// Raised when the user presses the save button.
		/// </summary>
		event EventHandler OnSaveButtonPressed;

		/// <summary>
		/// Sets the selected state for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetItemSelected(ushort index, bool selected);
	}
}
