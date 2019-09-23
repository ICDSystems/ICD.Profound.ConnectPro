using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts
{
	public interface IWtcContactListView : IUiView
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
		IEnumerable<IWtcReferencedContactView> GetContactViews(IViewFactory factory, ushort count);
		
		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IWtcReferencedSelectedContactView> GetSelectedContactViews(IViewFactory factory, ushort count);
	}
}