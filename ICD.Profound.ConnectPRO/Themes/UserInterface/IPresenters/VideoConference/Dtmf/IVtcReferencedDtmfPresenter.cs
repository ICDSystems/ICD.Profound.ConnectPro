using System;
using ICD.Connect.Conferencing.Participants;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf
{
	public interface IVtcReferencedDtmfPresenter : IUiPresenter<IVtcReferencedDtmfView>
	{
		/// <summary>
		/// Raised when the user presses the contact.
		/// </summary>
		event EventHandler OnPressed;

		IParticipant Source { get; set; }

		bool Selected { get; set; }
	}
}
