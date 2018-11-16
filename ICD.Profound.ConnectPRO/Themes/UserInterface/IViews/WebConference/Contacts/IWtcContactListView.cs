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
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IWtcReferencedContactView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}