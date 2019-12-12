using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference.ActiveConference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference.ActiveConference
{
	[ViewBinding(typeof(IActiveConferenceView))]
	public sealed partial class ActiveConferenceView : AbstractTouchDisplayView, IActiveConferenceView
	{
		/// <summary>
		/// Raised when the Show/Hide Camera button is pressed.
		/// </summary>
		public event EventHandler OnShowHideCameraButtonPressed;

		/// <summary>
		/// Raised when the End Video Meeting button is pressed.
		/// </summary>
		public event EventHandler OnEndMeetingButtonPressed;

		/// <summary>
		/// Raised when the Leave Video Meeting button is pressed.
		/// </summary>
		public event EventHandler OnLeaveMeetingButtonPressed;

		/// <summary>
		/// Raised when the Meeting Info button is pressed.
		/// </summary>
		public event EventHandler OnInviteButtonPressed;

		/// <summary>
		/// Raised when the lock button is pressed.
		/// </summary>
		public event EventHandler OnLockButtonPressed;

		private readonly List<IReferencedParticipantView> m_ChildList; 

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ActiveConferenceView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedParticipantView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnShowHideCameraButtonPressed = null;
			OnEndMeetingButtonPressed = null;
			OnLeaveMeetingButtonPressed = null;
			OnInviteButtonPressed = null;
			OnLockButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		public IEnumerable<IReferencedParticipantView> GetChildComponentViews(IViewFactory factory, ushort count)
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

		/// <summary>
		/// Sets the selected (locked) state of the lock button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetlockButtonSelected(bool selected)
		{
			m_LockButton.SetSelected(selected);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();
			
			m_EndMeetingButton.OnPressed += EndMeetingButtonOnPressed;
			m_LeaveMeetingButton.OnPressed += LeaveMeetingButtonOnPressed;
			m_ShowHideCameraButton.OnPressed += ShowHideCameraButtonOnPressed;
			m_InviteButton.OnPressed += InviteButtonOnPressed;
			m_LockButton.OnPressed += LockButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_EndMeetingButton.OnPressed -= EndMeetingButtonOnPressed;
			m_LeaveMeetingButton.OnPressed -= LeaveMeetingButtonOnPressed;
			m_ShowHideCameraButton.OnPressed -= ShowHideCameraButtonOnPressed;
			m_InviteButton.OnPressed -= InviteButtonOnPressed;
			m_LockButton.OnPressed -= LockButtonOnPressed;
		}

		private void LeaveMeetingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnLeaveMeetingButtonPressed.Raise(this);
		}

		private void EndMeetingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnEndMeetingButtonPressed.Raise(this);
		}

		private void ShowHideCameraButtonOnPressed(object sender, EventArgs e)
		{
			OnShowHideCameraButtonPressed.Raise(this);
		}

		private void InviteButtonOnPressed(object sender, EventArgs e)
		{
			OnInviteButtonPressed.Raise(this);
		}

		private void LockButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnLockButtonPressed.Raise(this);
		}

		#endregion
	}
}