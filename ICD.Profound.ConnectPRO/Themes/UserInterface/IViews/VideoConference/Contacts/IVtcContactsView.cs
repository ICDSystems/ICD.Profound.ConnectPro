using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcContactsView : IView
	{
		/// <summary>
		/// Raised when the user enters text in the text field.
		/// </summary>
		event EventHandler<StringEventArgs> OnTextEntered;

		/// <summary>
		/// Raised when the user presses the directory button.
		/// </summary>
		event EventHandler OnDirectoryButtonPressed;

		/// <summary>
		/// Raised when the user presses the favourites button.
		/// </summary>
		event EventHandler OnFavoritesButtonPressed;

		/// <summary>
		/// Raised when the user presses the recent button.
		/// </summary>
		event EventHandler OnRecentButtonPressed;

		/// <summary>
		/// Raised when the user presses the call button.
		/// </summary>
		event EventHandler OnCallButtonPressed;

		/// <summary>
		/// Raised when the user presses the hangup button.
		/// </summary>
		event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Raised when the user presses the back button.
		/// </summary>
		event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Raised when the user presses the home button.
		/// </summary>
		event EventHandler OnHomeButtonPressed;

		/// <summary>
		/// Raised when the user presses the search button.
		/// </summary>
		event EventHandler OnSearchButtonPressed;

		/// <summary>
		/// Raised when the user presses the manual dial button.
		/// </summary>
		event EventHandler OnManualDialButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IVtcReferencedContactsView> GetChildComponentViews(IViewFactory factory, ushort count);

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
		/// Sets the selected state of the recent button.
		/// </summary>
		/// <param name="selected"></param>
		void SetRecentButtonSelected(bool selected);

		/// <summary>
		/// Sets the enabled state of the call button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetCallButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the hangup button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetHangupButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the text for the searchbar.
		/// </summary>
		/// <param name="text"></param>
		void SetSearchBarText(string text);
	}
}
