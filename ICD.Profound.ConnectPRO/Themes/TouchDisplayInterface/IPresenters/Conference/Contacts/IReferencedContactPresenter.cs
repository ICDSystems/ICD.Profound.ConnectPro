using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts
{
	public interface IReferencedContactPresenter : ITouchDisplayPresenter<IReferencedContactView>
	{
		/// <summary>
		/// Raised when the user presses the contact.
		/// </summary>
		event EventHandler OnPressed;

		/// <summary>
		/// Raised when the online status changes.
		/// </summary>
		event EventHandler<OnlineStateEventArgs> OnOnlineStateChanged;

		/// <summary>
		/// Raised when the favorite button is pressed.
		/// </summary>
		event EventHandler OnFavoriteButtonPressed;

		/// <summary>
		/// Gets/sets the contact for this presenter.
		/// </summary>
		IContact Contact { get; set; }

		/// <summary>
		/// Gets/sets the favorite state for this contact.
		/// </summary>
		bool IsFavorite { get; set; }
	}
}