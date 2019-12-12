using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference
{
	public interface IActiveConferenceView : ITouchDisplayView
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
		/// Raised when the lock button is pressed.
		/// </summary>
		event EventHandler OnLockButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedParticipantView> GetChildComponentViews(IViewFactory factory, ushort count);

		void SetEndMeetingButtonEnabled(bool enabled);

		void SetLeaveMeetingButtonEnabled(bool enabled);

		void SetNoParticipantsLabelVisibility(bool visible);

		void SetInviteButtonVisibility(bool visible);

		void SetMeetingNumberLabelVisibility(bool enabled);

		void SetMeetingNumberLabelText(string text);

		void SetShowHideCameraButtonState(bool cameraEnabled);

		/// <summary>
		/// Sets the selected (locked) state of the lock button.
		/// </summary>
		/// <param name="selected"></param>
		void SetlockButtonSelected(bool selected);
	}
}