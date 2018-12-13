﻿using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	public sealed partial class WtcReferencedDirectoryItemView : AbstractComponentView, IWtcReferencedDirectoryItemView
	{
		public event EventHandler OnContactPressed;

		private ushort MODE_ONLINE = 0;
		private ushort MODE_AWAY = 1;
		private ushort MODE_BUSY = 2;

		public WtcReferencedDirectoryItemView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public override void Dispose()
		{
			OnContactPressed = null;

			base.Dispose();
		}

		public void SetContactName(string name)
		{
			m_ContactNameText.SetLabelText(name);
		}

		public void SetButtonSelected(bool selected)
		{
			m_ContactButton.SetSelected(selected);
		}

		public void SetAvatarImagePath(string url)
		{
			m_AvatarImage.SetImageUrl(url);
		}

		public void SetAvatarImageVisibility(bool visible)
		{
			m_AvatarImage.Show(visible);
		}

		public void SetOnlineStateMode(eOnlineState state)
		{
			switch (state)
			{
				case eOnlineState.Online:
					m_OnlineStateButton.SetMode(MODE_ONLINE);
					m_OnlineStateButton.Show(true);
					break;
				case eOnlineState.Away:
					m_OnlineStateButton.SetMode(MODE_AWAY);
					m_OnlineStateButton.Show(true);
					break;
				case eOnlineState.Busy:
				case eOnlineState.DoNotDisturb:
					m_OnlineStateButton.SetMode(MODE_BUSY);
					m_OnlineStateButton.Show(true);
					break;
				case eOnlineState.Offline:
				case eOnlineState.Unknown:
					m_OnlineStateButton.Show(false);
					break;

				default:
					throw new ArgumentOutOfRangeException("state");
			}
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ContactButton.OnPressed += ContactButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ContactButton.OnPressed -= ContactButtonOnOnPressed;
		}

		private void ContactButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnContactPressed.Raise(this);
		}

		#endregion
	}
}