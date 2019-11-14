using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts
{
	public interface IWtcReferencedContactPresenter : IUiPresenter<IWtcReferencedContactView>
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
		/// Raised when the contact is stored/removed from favorites.
		/// </summary>
		event EventHandler<BoolEventArgs> OnIsFavoritedStateChanged;

		/// <summary>
		/// Gets/sets the contact for this presenter.
		/// </summary>
		IContact Contact { get; set; }
	}
}