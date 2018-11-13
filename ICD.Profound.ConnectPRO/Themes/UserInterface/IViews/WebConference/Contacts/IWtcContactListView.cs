using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts
{
	public interface IWtcContactListView : IView
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