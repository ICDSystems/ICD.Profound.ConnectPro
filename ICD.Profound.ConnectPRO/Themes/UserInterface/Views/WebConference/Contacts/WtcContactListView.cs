using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	public sealed partial class WtcContactListView : AbstractUiView, IWtcContactListView
	{
		public event EventHandler OnInviteParticipantButtonPressed;
		public event EventHandler OnBackButtonPressed;

		private readonly List<IWtcReferencedDirectoryItemView> m_ChildList;

		public WtcContactListView(ISigInputOutput panel, ConnectProTheme theme) 
			: base(panel, theme)
		{
			m_ChildList = new List<IWtcReferencedDirectoryItemView>();
		}

		public override void Dispose()
		{
			OnInviteParticipantButtonPressed = null;
			OnBackButtonPressed = null;

			base.Dispose();
		}

		public IEnumerable<IWtcReferencedDirectoryItemView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ContactList, m_ChildList, count);
		}

		public void SetBackButtonEnabled(bool enabled)
		{
			m_BackButton.Enable(enabled);
		}

		public void SetInviteParticipantButtonEnabled(bool enabled)
		{
			m_InviteParticipantButton.Enable(enabled);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_InviteParticipantButton.OnPressed += InviteParticipantButtonOnPressed;
			m_BackButton.OnPressed += BackButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_InviteParticipantButton.OnPressed -= InviteParticipantButtonOnPressed;
			m_BackButton.OnPressed -= BackButtonOnPressed;
		}

		private void InviteParticipantButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnInviteParticipantButtonPressed.Raise(this);
		}

		private void BackButtonOnPressed(object sender, EventArgs e)
		{
			OnBackButtonPressed.Raise(this);
		}

		#endregion
	}
}