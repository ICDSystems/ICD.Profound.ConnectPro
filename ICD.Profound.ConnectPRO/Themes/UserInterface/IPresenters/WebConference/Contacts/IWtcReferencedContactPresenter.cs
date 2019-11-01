using System;
using ICD.Connect.Conferencing.Contacts;
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
		/// Gets/sets the contact for this presenter.
		/// </summary>
		IContact Contact { get; set; }
	}
}