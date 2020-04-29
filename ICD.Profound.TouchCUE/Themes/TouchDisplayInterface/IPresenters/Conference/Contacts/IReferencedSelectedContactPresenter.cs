using System;
using ICD.Connect.Conferencing.Contacts;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts
{
	public interface IReferencedSelectedContactPresenter : ITouchDisplayPresenter<IReferencedSelectedContactView>
	{
		event EventHandler OnRemoveContact;

		IContact Contact { get; set; }
	}
}