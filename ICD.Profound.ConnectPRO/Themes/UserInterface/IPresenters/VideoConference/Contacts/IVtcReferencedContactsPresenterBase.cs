using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts
{
	public interface IVtcReferencedContactsPresenterBase : IUiPresenter<IVtcReferencedContactsView>, IVtcPresenter
	{
		/// <summary>
		/// Called when the user presses the contact button.
		/// </summary>
		event EventHandler OnPressed;

		bool Selected { get; set; }

		bool HideFavoriteIcon { get; set; }

		void SetModel(object model);

		void Dial();
	}
}
