using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	[ViewBinding(typeof(IWtcContactListView))]
	public sealed partial class WtcContactListView : AbstractUiView, IWtcContactListView
	{
		public event EventHandler OnInviteParticipantButtonPressed;
		public event EventHandler OnSearchButtonPressed;

		private readonly List<IWtcReferencedContactView> m_ContactViewList;
		private readonly List<IWtcReferencedSelectedContactView> m_SelectedContactViewList;

		public WtcContactListView(ISigInputOutput panel, ConnectProTheme theme) 
			: base(panel, theme)
		{
			m_ContactViewList = new List<IWtcReferencedContactView>();
			m_SelectedContactViewList = new List<IWtcReferencedSelectedContactView>();
		}

		public override void Dispose()
		{
			OnInviteParticipantButtonPressed = null;
			OnSearchButtonPressed = null;

			base.Dispose();
		}

		public IEnumerable<IWtcReferencedContactView> GetContactViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ContactList, m_ContactViewList, count);
		}
		public IEnumerable<IWtcReferencedSelectedContactView> GetSelectedContactViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_SelectedContactList, m_SelectedContactViewList, count);
		}

		public void SetSearchButtonEnabled(bool enabled)
		{
			m_SearchButton.Enable(enabled);
		}

		public void SetInviteParticipantButtonEnabled(bool enabled)
		{
			m_InviteParticipantButton.Enable(enabled);
		}

		public void SetContactListLabelText(string text)
		{
			m_ContactListLabel.SetLabelText(text);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_InviteParticipantButton.OnPressed += InviteParticipantButtonOnPressed;
			m_SearchButton.OnPressed += SearchButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_InviteParticipantButton.OnPressed -= InviteParticipantButtonOnPressed;
			m_SearchButton.OnPressed -= SearchButtonOnPressed;
		}

		private void InviteParticipantButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnInviteParticipantButtonPressed.Raise(this);
		}

		private void SearchButtonOnPressed(object sender, EventArgs e)
		{
			OnSearchButtonPressed.Raise(this);
		}

		#endregion
	}
}