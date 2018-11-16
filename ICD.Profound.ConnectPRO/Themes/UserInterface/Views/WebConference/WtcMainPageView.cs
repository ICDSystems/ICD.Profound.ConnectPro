using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public sealed partial class WtcMainPageView : AbstractUiView, IWtcMainPageView
	{
		public event EventHandler OnMeetNowButtonPressed;
		public event EventHandler OnContactsButtonPressed;
		public event EventHandler OnJoinByIdButtonPressed;

		public WtcMainPageView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		#region Methods

		public void SetMeetNowButtonEnabled(bool enabled)
		{
			m_MeetNowButton.Enable(enabled);
		}

		public void SetJoinByIdButtonEnabled(bool enabled)
		{
			m_JoinByIdButton.Enable(enabled);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_MeetNowButton.OnPressed += MeetNowButtonOnOnPressed;
			m_ContactsButton.OnPressed += ContactsButtonOnOnPressed;
			m_JoinByIdButton.OnPressed += JoinByIdButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_MeetNowButton.OnPressed -= MeetNowButtonOnOnPressed;
			m_ContactsButton.OnPressed -= ContactsButtonOnOnPressed;
			m_JoinByIdButton.OnPressed -= JoinByIdButtonOnOnPressed;
		}

		private void JoinByIdButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnJoinByIdButtonPressed.Raise(this);
		}

		private void ContactsButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnContactsButtonPressed.Raise(this);
		}

		private void MeetNowButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnMeetNowButtonPressed.Raise(this);
		}

		#endregion
	}
}