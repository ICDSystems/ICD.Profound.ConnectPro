using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.Contacts
{
	public interface IContactListView : ITouchDisplayView
	{
		/// <summary>
		/// Raised when Invite Participant button is pressed.
		/// </summary>
		event EventHandler OnInviteParticipantButtonPressed;

		/// <summary>
		/// Raised when the Search button is pressed.
		/// </summary>
		event EventHandler OnSearchButtonPressed;

		/// <summary>
		/// Raised when the Favorites button is pressed.
		/// </summary>
		event EventHandler OnFavoritesButtonPressed;

		/// <summary>
		/// Sets the enabled state of the Invite Participant button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetInviteParticipantButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Search button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetSearchButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the text for the label above the contact list.
		/// </summary>
		/// <param name="text"></param>
		void SetContactListLabelText(string text);

		/// <summary>
		/// Sets the visibility of the no contacts selected label.
		/// </summary>
		/// <param name="show"></param>
		void ShowNoContactsSelectedLabel(bool show);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedContactView> GetContactViews(IViewFactory factory, ushort count);
		
		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedSelectedContactView> GetSelectedContactViews(IViewFactory factory, ushort count);

		/// <summary>
		/// Sets the selected state of the favorite button.
		/// </summary>
		/// <param name="selected"></param>
		void SetFavoritesButtonSelected(bool selected);
	}
}