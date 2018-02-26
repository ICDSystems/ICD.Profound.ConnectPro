using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcReferencedContactsView : AbstractComponentView, IVtcReferencedContactsView
	{
		public event EventHandler OnButtonPressed;
		public event EventHandler OnFavoriteButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		public VtcReferencedContactsView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		/// <summary>
		/// Sets the name of the contact.
		/// </summary>
		/// <param name="name"></param>
		public void SetContactName(string name)
		{
			m_ContactNameLabel.SetLabelText(name);
		}

		/// <summary>
		/// Sets the favorite state of the contact.
		/// </summary>
		/// <param name="favorite"></param>
		public void SetIsFavorite(bool favorite)
		{
			m_FavoriteButton.SetSelected(favorite);
		}

		/// <summary>
		/// Sets the visibility of the favorite button.
		/// </summary>
		/// <param name="favoriteVisible"></param>
		public void SetFavoriteButtonVisible(bool favoriteVisible)
		{
			m_FavoriteButton.Show(favoriteVisible);
		}

		/// <summary>
		/// Sets the selected state of the view.
		/// </summary>
		/// <param name="selected"></param>
		public void SetSelected(bool selected)
		{
			m_BackgroundButton.SetSelected(selected);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_BackgroundButton.OnPressed += BackgroundButtonOnPressed;
			m_FavoriteButton.OnPressed += FavoriteButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_BackgroundButton.OnPressed -= BackgroundButtonOnPressed;
			m_FavoriteButton.OnPressed -= FavoriteButtonOnPressed;
		}

		private void BackgroundButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnButtonPressed.Raise(this);
		}

		private void FavoriteButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnFavoriteButtonPressed.Raise(this);
		}

		#endregion
	}
}
