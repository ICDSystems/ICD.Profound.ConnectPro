using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedContactsPresenterBase : IPresenter<IVtcReferencedContactsView>
	{
		/// <summary>
		/// Called when the user presses the contact button.
		/// </summary>
		event EventHandler OnPressed;

		bool Selected { get; set; }

		void SetModel(object model);
		void Dial();
	}
}
