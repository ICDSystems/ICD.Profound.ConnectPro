using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	[ViewBinding(typeof(IWtcActiveMeetingView))]
	public sealed partial class WtcActiveMeetingView : AbstractUiView, IWtcActiveMeetingView
	{
		public event EventHandler OnShowHideCameraButtonPressed;
		public event EventHandler OnEndMeetingButtonPressed;
		public event EventHandler OnLeaveMeetingButtonPressed;
		public event EventHandler OnInviteButtonPressed;

		private readonly List<IWtcReferencedParticipantView> m_ChildList; 

		public WtcActiveMeetingView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IWtcReferencedParticipantView>();
		}

		public IEnumerable<IWtcReferencedParticipantView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ParticipantList, m_ChildList, count);
		}

		public void SetEndMeetingButtonEnabled(bool enabled)
		{
			m_EndMeetingButton.Enable(enabled);
		}

		public void SetLeaveMeetingButtonEnabled(bool enabled)
		{
			m_LeaveMeetingButton.Enable(enabled);
		}

		public void SetNoParticipantsLabelVisibility(bool visible)
		{
			m_NoParticipantsLabel.Show(visible);
		}

		public void SetInviteButtonVisibility(bool visible)
		{
			m_InviteButton.Show(visible);
		}

		public void SetMeetingNumberLabelVisibility(bool visible)
		{
			m_MeetingNumberLabel.Show(visible);
		}

		public void SetMeetingNumberLabelText(string text)
		{
			m_MeetingNumberLabel.SetLabelText(text);
		}

		/// <summary>
		/// Sets the state of the Show/Hide Camera button.
		/// If true, "Hide" is displayed. If false, "Show" is displayed.
		/// </summary>
		/// <param name="cameraEnabled"></param>
		public void SetShowHideCameraButtonState(bool cameraEnabled)
		{
			m_ShowHideCameraButton.SetLabelTextAtJoin(m_ShowHideCameraButton.DigitalLabelJoins.First(), cameraEnabled);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();
			
			m_EndMeetingButton.OnPressed += EndMeetingButtonOnOnPressed;
			m_LeaveMeetingButton.OnPressed += LeaveMeetingButtonOnOnPressed;
			m_ShowHideCameraButton.OnPressed += ShowHideCameraButtonOnOnPressed;
			m_InviteButton.OnPressed += InviteButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_EndMeetingButton.OnPressed -= EndMeetingButtonOnOnPressed;
			m_LeaveMeetingButton.OnPressed -= LeaveMeetingButtonOnOnPressed;
			m_ShowHideCameraButton.OnPressed -= ShowHideCameraButtonOnOnPressed;
			m_InviteButton.OnPressed -= InviteButtonOnOnPressed;
		}

		private void LeaveMeetingButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnLeaveMeetingButtonPressed.Raise(this);
		}

		private void EndMeetingButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnEndMeetingButtonPressed.Raise(this);
		}

		private void ShowHideCameraButtonOnOnPressed(object sender, EventArgs e)
		{
			OnShowHideCameraButtonPressed.Raise(this);
		}

		private void InviteButtonOnOnPressed(object sender, EventArgs e)
		{
			OnInviteButtonPressed.Raise(this);
		}

		#endregion
	}
}