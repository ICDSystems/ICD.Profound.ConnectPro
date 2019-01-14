using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcLeftMenuView : IUiView
	{
		/// <summary>
		/// Raised when the user presses a button.
		/// </summary>
		event EventHandler<UShortEventArgs> OnButtonPressed;

		/// <summary>
		/// Sets the label for a button in the list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		void SetButtonLabel(ushort index, string label);

		/// <summary>
		/// Sets the icon for a button in the list
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		void SetButtonIcon(ushort index, string icon);

		/// <summary>
		/// Sets the visibility of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="visible"></param>
		void SetButtonVisible(ushort index, bool visible);

		/// <summary>
		/// Sets the enabled state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="enabled"></param>
		void SetButtonEnabled(ushort index, bool enabled);

		/// <summary>
		/// Sets the selected state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the number of source buttons.
		/// </summary>
		/// <param name="count"></param>
		void SetButtonCount(ushort count);

		void SetActiveMeetingIndicatorMode(bool inMeeting);
	}
}