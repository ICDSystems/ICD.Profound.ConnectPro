using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts
{
	public interface IWtcReferencedSelectedContactView : IUiView
	{
		/// <summary>
		/// Raised when the contact is pressed.
		/// </summary>
		event EventHandler OnRemovePressed;

		/// <summary>
		/// Sets the text for the contact's name.
		/// </summary>
		/// <param name="name"></param>
		void SetContactName(string name);

		void SetAvatarImagePath(string url);

		void SetAvatarImageVisibility(bool visible);
	}
}