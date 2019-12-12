using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference
{
	public interface IReferencedParticipantView : ITouchDisplayView
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

		void SetAvatarImageVisibility(bool visible);

		void SetAvatarImagePath(string url);

		void SetMuteIconVisibility(bool visible);
	}
}