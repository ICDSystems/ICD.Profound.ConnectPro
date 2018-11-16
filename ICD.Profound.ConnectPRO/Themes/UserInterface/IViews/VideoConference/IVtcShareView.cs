using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcShareView : IUiView
	{
		/// <summary>
		/// Raised when the user presses a source button.
		/// </summary>
		event EventHandler<UShortEventArgs> OnSourceButtonPressed;

		/// <summary>
		/// Raised when the user presses the share button.
		/// </summary>
		event EventHandler OnShareButtonPressed;

		/// <summary>
		/// Sets the label for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		void SetButtonLabel(ushort index, string label);

		/// <summary>
		/// Sets the icon for the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		void SetButtonIcon(ushort index, string icon);

		/// <summary>
		/// Sets the selection state of the button at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="selected"></param>
		void SetButtonSelected(ushort index, bool selected);

		/// <summary>
		/// Sets the number of source buttons.
		/// </summary>
		/// <param name="count"></param>
		void SetButtonCount(ushort count);

		/// <summary>
		/// Sets the enabled state of the share button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetShareButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the selected state of the share button.
		/// </summary>
		/// <param name="selected"></param>
		void SetShareButtonSelected(bool selected);
	}
}
