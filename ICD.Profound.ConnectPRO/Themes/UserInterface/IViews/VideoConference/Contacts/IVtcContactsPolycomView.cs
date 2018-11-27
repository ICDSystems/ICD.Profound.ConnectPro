using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts
{
	public interface IVtcContactsPolycomView : IVtcContactsView
	{
		/// <summary>
		/// Raised when the user presses the directory button.
		/// </summary>
		event EventHandler OnNavigationButtonPressed;

		/// <summary>
		/// Raised when the user presses the favourites button.
		/// </summary>
		event EventHandler OnLocalButtonPressed;

		event EventHandler OnDPadUpButtonPressed;
		event EventHandler OnDPadDownButtonPressed;
		event EventHandler OnDPadLeftButtonPressed;
		event EventHandler OnDPadRightButtonPressed;
		event EventHandler OnDPadSelectButtonPressed;
		
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

		void SetBackButtonVisible(bool visible);
		void SetHomeButtonVisible(bool visible);
		void SetDirectoryButtonVisible(bool visible);
	}
}