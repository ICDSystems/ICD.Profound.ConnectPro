using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts
{
	public interface IWtcReferencedSelectedContactPresenter : IUiPresenter<IWtcReferencedSelectedContactView>
	{
		event EventHandler OnRemoveContact;

		IContact Contact { get; set; }
	}
}