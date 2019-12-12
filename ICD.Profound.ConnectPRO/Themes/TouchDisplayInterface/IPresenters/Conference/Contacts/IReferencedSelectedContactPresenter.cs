using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts
{
	public interface IReferencedSelectedContactPresenter : ITouchDisplayPresenter<IReferencedSelectedContactView>
	{
		event EventHandler OnRemoveContact;

		IContact Contact { get; set; }
	}
}