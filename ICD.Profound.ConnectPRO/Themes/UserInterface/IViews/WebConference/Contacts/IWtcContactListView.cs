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
		/// Raised when the Back button is pressed.
		/// </summary>
		event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Sets the enabled state of the Invite Participant button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetInviteParticipantButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Back button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetBackButtonEnabled(bool enabled);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IWtcReferencedDirectoryItemView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}