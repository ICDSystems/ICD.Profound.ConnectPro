using System;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts
{
	public interface IReferencedSelectedContactView : ITouchDisplayView
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