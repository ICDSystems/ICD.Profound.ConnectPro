using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcActiveMeetingToggleView : IView
	{
		/// <summary>
		/// Raised when the button is pressed.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// When true shows the "Active Meeting" button, otherwise shows the "Call" button.
		/// </summary>
		/// <remarks>
		/// Not sure why the panel was designed this way (separate toggle buttons for active meeting and contacts), but meh.
		/// </remarks>
		/// <param name="mode"></param>
		void SetActiveMeetingMode(bool mode);

		/// <summary>
		/// Sets the visibility of the button and label.
		/// </summary>
		/// <param name="visible"></param>
		void SetButtonVisible(bool visible);
	}
}