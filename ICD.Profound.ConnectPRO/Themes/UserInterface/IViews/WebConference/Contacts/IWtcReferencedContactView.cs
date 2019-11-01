using System;
using ICD.Connect.Conferencing.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts
{
	public interface IWtcReferencedContactView : IUiView
	{
		/// <summary>
		/// Raised when the contact is pressed.
		/// </summary>
		event EventHandler OnContactPressed;

		/// <summary>
		/// Raised when the favorite button is pressed.
		/// </summary>
		event EventHandler OnFavoriteButtonPressed;

		/// <summary>
		/// Sets the text for the contact's name.
		/// </summary>
		/// <param name="name"></param>
		void SetContactName(string name);

		/// <summary>
		/// Sets the path to the avatar image.
		/// </summary>
		/// <param name="url"></param>
		void SetAvatarImagePath(string url);

		/// <summary>
		/// Sets the visibility of the avatar image.
		/// </summary>
		/// <param name="visible"></param>
		void SetAvatarImageVisibility(bool visible);

		/// <summary>
		/// Sets the online state.
		/// </summary>
		/// <param name="state"></param>
		void SetOnlineStateMode(eOnlineState state);

		/// <summary>
		/// Sets the selected state for the favorite button.
		/// </summary>
		/// <param name="selected"></param>
		void SetFavoriteButtonSelected(bool selected);
	}
}