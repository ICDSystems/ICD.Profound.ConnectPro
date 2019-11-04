using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu
{
	public interface IWtcLeftMenuButtonModel
	{
		/// <summary>
		/// Raised when the state changes.
		/// </summary>
		event EventHandler<BoolEventArgs> OnStateChanged; 

		/// <summary>
		/// Gets the label for the button.
		/// </summary>
		string Label { get; }

		/// <summary>
		/// Gets the icon for the button.
		/// </summary>
		string Icon { get; }

		/// <summary>
		/// Gets the feedback state for the light.
		/// </summary>
		bool State { get; }

		/// <summary>
		/// Returns true if the button has a visible state.
		/// </summary>
		bool HasState { get; }

		/// <summary>
		/// Returns true if the button should be enabled.
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// Called to handle the button press.
		/// </summary>
		void HandlePress();
	}
}