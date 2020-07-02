using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference
{
	public interface IWtcShareView : IUiView
	{
		event EventHandler OnShareButtonPressed;

		event EventHandler<UShortEventArgs> OnSourceButtonPressed;

		void SetSwipeLabelsVisible(bool visible);

		/// <summary>
		/// Sets the labels for the buttons in the list.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="label"></param>
		void SetButtonLabel(ushort index, string label);

		/// <summary>
		/// Sets the icons for the buttons in the list
		/// </summary>
		/// <param name="index"></param>
		/// <param name="icon"></param>
		void SetButtonIcon(ushort index, string icon);

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