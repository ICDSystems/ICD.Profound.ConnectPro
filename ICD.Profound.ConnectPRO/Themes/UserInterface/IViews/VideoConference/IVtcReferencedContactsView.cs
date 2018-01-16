using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcReferencedContactsView : IView
	{
		/// <summary>
		/// Raised when the user presses the contact.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Raised when the user presses the favorite button.
		/// </summary>
		event EventHandler OnFavoriteButtonPressed;

		/// <summary>
		/// Sets the name of the contact.
		/// </summary>
		/// <param name="name"></param>
		void SetContactName(string name);

		/// <summary>
		/// Sets the favorite state of the contact.
		/// </summary>
		/// <param name="favorite"></param>
		void SetIsFavorite(bool favorite);
	}
}
