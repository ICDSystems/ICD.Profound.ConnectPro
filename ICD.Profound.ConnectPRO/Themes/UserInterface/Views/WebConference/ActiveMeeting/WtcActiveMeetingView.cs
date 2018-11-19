using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	public sealed partial class WtcActiveMeetingView : AbstractUiView, IWtcActiveMeetingView
	{
		public event EventHandler OnKickParticipantButtonPressed;
		public event EventHandler OnMuteParticipantButtonPressed;
		public event EventHandler OnShowHideCameraButtonPressed;
		public event EventHandler OnEndMeetingButtonPressed;
		public event EventHandler OnLeaveMeetingButtonPressed;

		private readonly List<IWtcReferencedParticipantView> m_ChildList; 

		public WtcActiveMeetingView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
			m_ChildList = new List<IWtcReferencedParticipantView>();
		}

		public IEnumerable<IWtcReferencedParticipantView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ParticipantList, m_ChildList, count);
		}

		public void SetKickParticipantButtonEnabled(bool enabled)
		{
			m_KickParticipantButton.Enable(enabled);
		}

		public void SetMuteParticipantButtonEnabled(bool enabled)
		{
			m_MuteParticipantButton.Enable(enabled);
		}

		public void SetEndMeetingButtonEnabled(bool enabled)
		{
			m_EndMeetingButton.Enable(enabled);
		}

		public void SetLeaveMeetingButtonEnabled(bool enabled)
		{
			m_LeaveMeetingButton.Enable(enabled);
		}

		public void SetMeetingIdLabelVisibility(bool visible)
		{
			m_MeetingIdLabel.Show(visible);
		}

		public void SetMeetingIdLabelText(string text)
		{
			m_MeetingIdLabel.SetLabelText(text);
		}

		public void SetCallInLabelVisibility(bool visible)
		{
			m_CallInLabel.Show(visible);
		}

		public void SetCallInLabelText(string text)
		{
			m_CallInLabel.SetLabelText(text);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_KickParticipantButton.OnPressed += KickParticipantButtonOnOnPressed;
			m_MuteParticipantButton.OnPressed += MuteParticipantButtonOnOnPressed;
			m_EndMeetingButton.OnPressed += EndMeetingButtonOnOnPressed;
			m_LeaveMeetingButton.OnPressed += LeaveMeetingButtonOnOnPressed;
			m_ShowHideCameraButton.OnPressed += ShowHideCameraButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_KickParticipantButton.OnPressed -= KickParticipantButtonOnOnPressed;
			m_MuteParticipantButton.OnPressed -= MuteParticipantButtonOnOnPressed;
			m_EndMeetingButton.OnPressed -= EndMeetingButtonOnOnPressed;
			m_LeaveMeetingButton.OnPressed -= LeaveMeetingButtonOnOnPressed;
			m_ShowHideCameraButton.OnPressed -= ShowHideCameraButtonOnOnPressed;
		}

		private void LeaveMeetingButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnLeaveMeetingButtonPressed.Raise(this);
		}

		private void EndMeetingButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnEndMeetingButtonPressed.Raise(this);
		}

		private void MuteParticipantButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnMuteParticipantButtonPressed.Raise(this);
		}

		private void KickParticipantButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnKickParticipantButtonPressed.Raise(this);
		}

		private void ShowHideCameraButtonOnOnPressed(object sender, EventArgs e)
		{
			OnShowHideCameraButtonPressed.Raise(this);
		}

		#endregion
	}
}