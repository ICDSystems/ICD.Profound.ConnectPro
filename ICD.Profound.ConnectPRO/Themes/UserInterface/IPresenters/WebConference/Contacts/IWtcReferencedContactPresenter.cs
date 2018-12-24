using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts
{
	public interface IWtcReferencedContactPresenter : IUiPresenter<IWtcReferencedContactView>
	{
		event EventHandler OnPressed;

		IContact Contact { get; set; }

		bool Selected { get; set; }
	}
}