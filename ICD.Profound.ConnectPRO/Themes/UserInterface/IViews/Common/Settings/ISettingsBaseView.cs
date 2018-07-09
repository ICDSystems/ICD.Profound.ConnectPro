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
		/// Sets the list item at the given index as active.
		/// </summary>
		/// <param name="index"></param>
		void SetActiveListItem(ushort index);
	}
}
