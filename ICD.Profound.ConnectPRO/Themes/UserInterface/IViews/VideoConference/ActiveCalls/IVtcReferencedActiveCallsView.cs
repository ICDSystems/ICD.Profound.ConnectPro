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
		/// Sets the label text for the contact.
		/// </summary>
		/// <param name="label"></param>
		void SetLabel(string label);

		/// <summary>
		/// Sets the visibility of the hangup button.
		/// </summary>
		/// <param name="visible"></param>
		void SetHangupButtonVisible(bool visible);
	}
}
