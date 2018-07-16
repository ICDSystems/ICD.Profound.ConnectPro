using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts
{
	public interface IVtcContactsPolycomView : IView
	{
		/// <summary>
		/// Raised when the user presses the directory button.
		/// </summary>
		event EventHandler OnDPadButtonPressed;

		/// <summary>
		/// Raised when the user presses the favourites button.
		/// </summary>
		event EventHandler OnLocalButtonPressed;

		/// <summary>
		/// Raised when the user presses the recent button.
		/// </summary>
		event EventHandler OnRecentButtonPressed;

		/// <summary>
		/// Raised when the user presses the call button.
		/// </summary>
		event EventHandler OnCallButtonPressed;

		/// <summary>
		/// Raised when the user presses the hangup button.
		/// </summary>
		event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Raised when the user presses the back button.
		/// </summary>
		event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Raised when the user presses the home button.
		/// </summary>
		event EventHandler OnHomeButtonPressed;

		/// <summary>
		/// Raised when the user presses the search button.
		/// </summary>
		event EventHandler OnDirectoryButtonPressed;

		/// <summary>
		/// Raised when the user presses the manual dial button.
		/// </summary>
		event EventHandler OnManualDialButtonPressed;

		event EventHandler OnDPadUpButtonPressed;
		event EventHandler OnDPadDownButtonPressed;
		event EventHandler OnDPadLeftButtonPressed;
		event EventHandler OnDPadRightButtonPressed;
		event EventHandler OnDPadSelectButtonPressed;
		
		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IVtcReferencedContactsView> GetChildComponentViews(IViewFactory factory, ushort count);

		/// <summary>
		/// Sets the visibility of the DPad.
		/// </summary>
		/// <param name="visible"></param>
		void SetDPadVisible(bool visible);

		/// <summary>
		/// Sets the selected state of the directory button.
		/// </summary>
		/// <param name="selected"></param>
		void SetDPadButtonSelected(bool selected);

		/// <summary>
		/// Sets the selected state of the favorites button.
		/// </summary>
		/// <param name="selected"></param>
		void SetLocalButtonSelected(bool selected);

		/// <summary>
		/// Sets the selected state of the recent button.
		/// </summary>
		/// <param name="selected"></param>
		void SetRecentButtonSelected(bool selected);

		/// <summary>
		/// Sets the enabled state of the call button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetCallButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the hangup button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetHangupButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the visibility of the navigation buttons.
		/// </summary>
		/// <param name="visible"></param>
		void SetNavigationButtonsVisible(bool visible);
	}
}