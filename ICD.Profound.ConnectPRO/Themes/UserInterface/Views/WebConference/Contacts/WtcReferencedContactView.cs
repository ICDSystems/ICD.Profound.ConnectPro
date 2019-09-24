using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	[ViewBinding(typeof(IWtcReferencedContactView))]
	public sealed partial class WtcReferencedContactView : AbstractComponentView, IWtcReferencedContactView
	{
		public event EventHandler OnContactPressed;

		private const ushort MODE_ONLINE = 0;
		private const ushort MODE_AWAY = 1;
		private const ushort MODE_BUSY = 2;

		public WtcReferencedContactView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
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

			m_ContactButton.OnPressed += ContactButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ContactButton.OnPressed -= ContactButtonOnPressed;
		}

		private void ContactButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnContactPressed.Raise(this);
		}

		#endregion
	}
}