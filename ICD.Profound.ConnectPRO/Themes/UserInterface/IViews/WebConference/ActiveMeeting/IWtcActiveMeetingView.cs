﻿using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting
{
	public interface IWtcActiveMeetingView : IUiView
	{
		/// <summary>
		/// Raised when the Kick Participant button is pressed.
		/// </summary>
		event EventHandler OnKickParticipantButtonPressed;

		/// <summary>
		/// Raised when the Mute Participant button is pressed.
		/// </summary>
		event EventHandler OnMuteParticipantButtonPressed;

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
		event EventHandler OnMeetingInfoButtonPressed;

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="enabled"></param>
		void SetMeetingInfoButtonEnabled(bool enabled);

		void SetShowHideCameraButtonState(bool cameraEnabled);
	}
}