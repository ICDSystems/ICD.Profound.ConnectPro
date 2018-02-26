using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls
{
	public interface IVtcReferencedActiveCallsView : IView
	{
		/// <summary>
		/// Raised when the user presses the hangup button.
		/// </summary>
		event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Sets the name for the contact.
		/// </summary>
		/// <param name="name"></param>
		void SetContactName(string name);

		/// <summary>
		/// Sets the number for the contact.
		/// </summary>
		/// <param name="number"></param>
		void SetContactNumber(string number);
	}
}
