using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting
{
	public interface IWtcReferencedParticipantView : IView
	{
		/// <summary>
		/// Raised when the participant is pressed.
		/// </summary>
		event EventHandler OnParticipantPressed;

		/// <summary>
		/// Sets the text for the participant's name.
		/// </summary>
		/// <param name="name"></param>
		void SetParticipantName(string name);

		void SetButtonSelected(bool selected);
	}
}