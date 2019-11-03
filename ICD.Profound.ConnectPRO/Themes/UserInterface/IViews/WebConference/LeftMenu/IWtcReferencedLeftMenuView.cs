using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu
{
	public interface IWtcReferencedLeftMenuView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		event EventHandler OnButtonPressed;

		/// <summary>
		/// Sets the visibility of the status light.
		/// </summary>
		/// <param name="visible"></param>
		void SetStatusLightVisible(bool visible);

		/// <summary>
		/// Sets the text label for the button.
		/// </summary>
		/// <param name="label"></param>
		void SetLabelText(string label);

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		void SetIcon(string icon);

		/// <summary>
		/// Sets the status light state.
		/// </summary>
		/// <param name="state"></param>
		void SetStatusLightState(bool state);
	}
}