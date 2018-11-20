using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts
{
	public interface IWtcReferencedDirectoryItemView : IUiView
	{
		/// <summary>
		/// Raised when the contact is pressed.
		/// </summary>
		event EventHandler OnContactPressed;

		/// <summary>
		/// Sets the text for the contact's name.
		/// </summary>
		/// <param name="name"></param>
		void SetContactName(string name);

		void SetButtonSelected(bool selected);
	}
}