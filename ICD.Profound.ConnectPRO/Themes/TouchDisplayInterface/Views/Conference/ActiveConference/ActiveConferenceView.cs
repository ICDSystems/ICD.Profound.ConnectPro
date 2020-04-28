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
			OnInviteButtonPressed = null;
			OnLockButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		public IEnumerable<IReferencedParticipantView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ParticipantList, m_ChildList, count);
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
		/// Sets the selected (locked) state of the lock button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetLockButtonSelected(bool selected)
		{
			m_LockButton.SetSelected(selected);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_InviteButton.OnPressed += InviteButtonOnPressed;
			m_LockButton.OnPressed += LockButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_InviteButton.OnPressed -= InviteButtonOnPressed;
			m_LockButton.OnPressed -= LockButtonOnPressed;
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