using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	[ViewBinding(typeof(IWtcReferencedContactView))]
	public sealed partial class WtcReferencedContactView : AbstractComponentView, IWtcReferencedContactView
	{
		private const ushort MODE_OFFLINE = 0;
		private const ushort MODE_ONLINE = 1;
		private const ushort MODE_AWAY = 2;
		private const ushort MODE_BUSY = 3;

		/// <summary>
		/// Raised when the contact is pressed.
		/// </summary>
		public event EventHandler OnContactPressed;

		/// <summary>
		/// Raised when the favorite button is pressed.
		/// </summary>
		public event EventHandler OnFavoriteButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public WtcReferencedContactView(ISigInputOutput panel, IConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnContactPressed = null;
			OnFavoriteButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the text for the contact's name.
		/// </summary>
		/// <param name="name"></param>
		public void SetContactName(string name)
		{
			m_ContactNameText.SetLabelText(name);
		}

		/// <summary>
		/// Sets the path to the avatar image.
		/// </summary>
		/// <param name="url"></param>
		public void SetAvatarImagePath(string url)
		{
			m_AvatarImage.SetImageUrl(url);
		}

		/// <summary>
		/// Sets the visibility of the avatar image.
		/// </summary>
		/// <param name="visible"></param>
		public void SetAvatarImageVisibility(bool visible)
		{
			m_AvatarImage.Show(visible);
		}

		/// <summary>
		/// Sets the online state.
		/// </summary>
		/// <param name="state"></param>
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
					m_OnlineStateButton.SetMode(MODE_OFFLINE);
					m_OnlineStateButton.Show(true);
					break;
				case eOnlineState.Unknown:
					m_OnlineStateButton.SetMode(MODE_OFFLINE);
					m_OnlineStateButton.Show(false);
					break;

				default:
					throw new ArgumentOutOfRangeException("state");
			}
		}

		/// <summary>
		/// Sets the selected state for the favorite button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetFavoriteButtonSelected(bool selected)
		{
			m_FavoriteButton.SetSelected(selected);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ContactButton.OnPressed += ContactButtonOnPressed;
			m_FavoriteButton.OnPressed += FavoriteButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ContactButton.OnPressed -= ContactButtonOnPressed;
			m_FavoriteButton.OnPressed -= FavoriteButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the contact.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ContactButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnContactPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the favorite button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void FavoriteButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnFavoriteButtonPressed.Raise(this);
		}

		#endregion
	}
}