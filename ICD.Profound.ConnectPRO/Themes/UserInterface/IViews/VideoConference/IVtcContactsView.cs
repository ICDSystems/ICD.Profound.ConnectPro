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
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IVtcReferencedContactsView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
