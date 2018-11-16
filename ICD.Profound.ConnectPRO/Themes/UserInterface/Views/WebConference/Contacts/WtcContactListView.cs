using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	public sealed partial class WtcContactListView : AbstractUiView, IWtcContactListView
	{
		public event EventHandler OnInviteParticipantButtonPressed;

		private readonly List<IWtcReferencedContactView> m_ChildList;

		public WtcContactListView(ISigInputOutput panel, ConnectProTheme theme) 
			: base(panel, theme)
		{
			m_ChildList = new List<IWtcReferencedContactView>();
		}

		public override void Dispose()
		{
			OnInviteParticipantButtonPressed = null;

			base.Dispose();
		}

		public IEnumerable<IWtcReferencedContactView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ContactList, m_ChildList, count);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_InviteParticipantButton.OnPressed += InviteParticipantButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_InviteParticipantButton.OnPressed -= InviteParticipantButtonOnPressed;
		}

		private void InviteParticipantButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnInviteParticipantButtonPressed.Raise(this);
		}

		#endregion
	}
}