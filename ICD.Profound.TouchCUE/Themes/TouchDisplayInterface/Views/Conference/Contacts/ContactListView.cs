using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference.Contacts
{
	[ViewBinding(typeof(IContactListView))]
	public sealed partial class ContactListView : AbstractTouchDisplayView, IContactListView
	{
		/// <summary>
		/// Raised when Invite Participant button is pressed.
		/// </summary>
		public event EventHandler OnInviteParticipantButtonPressed;

		/// <summary>
		/// Raised when the Search button is pressed.
		/// </summary>
		public event EventHandler OnSearchButtonPressed;

		/// <summary>
		/// Raised when the Favorites button is pressed.
		/// </summary>
		public event EventHandler OnFavoritesButtonPressed;

		private readonly List<IReferencedContactView> m_ContactViewList;
		private readonly List<IReferencedSelectedContactView> m_SelectedContactViewList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ContactListView(ISigInputOutput panel, TouchCueTheme theme) 
			: base(panel, theme)
		{
			m_ContactViewList = new List<IReferencedContactView>();
			m_SelectedContactViewList = new List<IReferencedSelectedContactView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnInviteParticipantButtonPressed = null;
			OnSearchButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedContactView> GetContactViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ContactList, m_ContactViewList, count);
		}

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedSelectedContactView> GetSelectedContactViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_SelectedContactList, m_SelectedContactViewList, count);
		}

		/// <summary>
		/// Sets the selected state of the favorite button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetFavoritesButtonSelected(bool selected)
		{
			m_FavoritesButton.SetSelected(selected);
		}

		/// <summary>
		/// Sets the enabled state of the Search button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetSearchButtonEnabled(bool enabled)
		{
			m_SearchButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the enabled state of the Invite Participant button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetInviteParticipantButtonEnabled(bool enabled)
		{
			m_InviteParticipantButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the text for the label above the contact list.
		/// </summary>
		/// <param name="text"></param>
		public void SetContactListLabelText(string text)
		{
			m_ContactListLabel.SetLabelText(text);
		}

		/// <summary>
		/// Sets the visibility of the no contacts selected label.
		/// </summary>
		/// <param name="show"></param>
		public void ShowNoContactsSelectedLabel(bool show)
		{
			m_NoContactsSelectedLabel.Show(show);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_InviteParticipantButton.OnPressed += InviteParticipantButtonOnPressed;
			m_SearchButton.OnPressed += SearchButtonOnPressed;
			m_FavoritesButton.OnPressed += FavoritesButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_InviteParticipantButton.OnPressed -= InviteParticipantButtonOnPressed;
			m_SearchButton.OnPressed -= SearchButtonOnPressed;
			m_FavoritesButton.OnPressed -= FavoritesButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the invite participant button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void InviteParticipantButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnInviteParticipantButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the search button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SearchButtonOnPressed(object sender, EventArgs e)
		{
			OnSearchButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the favorites button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void FavoritesButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnFavoritesButtonPressed.Raise(this);
		}

		#endregion
	}
}