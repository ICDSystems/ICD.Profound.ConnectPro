using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting
{
	public interface IWtcActiveMeetingView : IUiView
	{
		/// <summary>
		/// Raised when the Show/Hide Camera button is pressed.
		/// </summary>
		event EventHandler OnShowHideCameraButtonPressed;

		/// <summary>
		/// Raised when the End Video Meeting button is pressed.
		/// </summary>
		event EventHandler OnEndMeetingButtonPressed;

		/// <summary>
		/// Raised when the Leave Video Meeting button is pressed.
		/// </summary>
		event EventHandler OnLeaveMeetingButtonPressed;

		/// <summary>
		/// Raised when the Meeting Info button is pressed.
		/// </summary>
		event EventHandler OnInviteButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IWtcReferencedParticipantView> GetChildComponentViews(IViewFactory factory, ushort count);

		void SetEndMeetingButtonEnabled(bool enabled);

		void SetLeaveMeetingButtonEnabled(bool enabled);

		void SetNoParticipantsLabelVisibility(bool visible);

		void SetInviteButtonVisibility(bool visible);

		void SetMeetingNumberLabelVisibility(bool enabled);

		void SetMeetingNumberLabelText(string text);

		void SetShowHideCameraButtonState(bool cameraEnabled);
	}
}