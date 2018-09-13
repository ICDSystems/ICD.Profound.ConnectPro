using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts
{
	public interface IVtcContactsNormalView : IVtcContactsView
	{
		/// <summary>
		/// Raised when the user presses the favourites button.
		/// </summary>
		event EventHandler OnFavoritesButtonPressed;

		/// <summary>
		/// Raised when the user presses the search button.
		/// </summary>
		event EventHandler OnSearchButtonPressed;

		/// <summary>
		/// Sets the selected state of the directory button.
		/// </summary>
		/// <param name="selected"></param>
		void SetDirectoryButtonSelected(bool selected);

		/// <summary>
		/// Sets the selected state of the favorites button.
		/// </summary>
		/// <param name="selected"></param>
		void SetFavoritesButtonSelected(bool selected);

		/// <summary>
		/// Sets the visibility of the navigation buttons.
		/// </summary>
		/// <param name="visible"></param>
		void SetNavigationButtonsVisible(bool visible);
	}
}
