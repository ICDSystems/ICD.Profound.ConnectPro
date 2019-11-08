using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Favorites;
using ICD.Connect.Conferencing.Zoom.Components.Directory;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	[PresenterBinding(typeof(IWtcReferencedContactPresenter))]
	public sealed class WtcReferencedContactPresenter : AbstractUiComponentPresenter<IWtcReferencedContactView>, IWtcReferencedContactPresenter
	{
		/// <summary>
		/// Raised when the user presses the contact.
		/// </summary>
		public event EventHandler OnPressed;

		/// <summary>
		/// Raised when the contact is stored/removed from favorites.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnIsFavoritedStateChanged;

		private readonly SafeCriticalSection m_RefreshSection;
		private IContact m_Contact;
		private bool m_IsFavorite;

		#region Properties

		/// <summary>
		/// Gets/sets the contact for this presenter.
		/// </summary>
		[CanBeNull]
		public IContact Contact
		{
			get { return m_Contact; }
			set
			{
				if (value == m_Contact)
					return;

				Unsubscribe(m_Contact);
				m_Contact = value;
				Subscribe(m_Contact);

				// Avoid the IsFavorite setter to avoid expensive refreshing
				m_IsFavorite = GetIsFavorite();

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the IsFavorite state.
		/// </summary>
		public bool IsFavorite
		{
			get { return m_IsFavorite; }
			private set
			{
				if (value == m_IsFavorite)
					return;

				m_IsFavorite = value;

				// Raise the event before refreshing because the contact browser may remove this referenced contact
				OnIsFavoritedStateChanged.Raise(this, new BoolEventArgs(m_IsFavorite));

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcReferencedContactPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;
			OnIsFavoritedStateChanged = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedContactView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetContactName(Contact == null ? "Missing Contact Name" : Contact.Name);

				// Online state indicator
				IContactWithOnlineState onlineContact = Contact as IContactWithOnlineState;
				view.SetOnlineStateMode(onlineContact == null ? eOnlineState.Offline : onlineContact.OnlineState);

				// Avatar
				ZoomContact zoomContact = Contact as ZoomContact;
				view.SetAvatarImageVisibility(zoomContact != null && !string.IsNullOrEmpty(zoomContact.AvatarUrl));
				view.SetAvatarImagePath(zoomContact == null ? null : zoomContact.AvatarUrl);

				// Favorite state
				view.SetFavoriteButtonSelected(IsFavorite);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Adds the contact as a favorite if not already a favorite, otherwise
		/// removes the contact from favorites.
		/// </summary>
		private void ToggleFavorite()
		{
			if (Contact == null)
				return;

			IConferenceManager conferenceManager = Room == null ? null : Room.ConferenceManager;
			IFavorites favorites = conferenceManager == null ? null : conferenceManager.Favorites;
			if (favorites == null)
				return;

			IsFavorite = favorites.ToggleFavorite(Contact);
		}

		/// <summary>
		/// Returns true if the current contact is stored as a favorite.
		/// </summary>
		/// <returns></returns>
		private bool GetIsFavorite()
		{
			if (Contact == null)
				return false;

			IConferenceManager conferenceManager = Room == null ? null : Room.ConferenceManager;
			IFavorites favorites = conferenceManager == null ? null : conferenceManager.Favorites;
			if (favorites == null)
				return false;

			return favorites.ContainsFavorite(Contact);
		}

		#endregion

		#region Contact Callbacks 

		/// <summary>
		/// Subscribe to the contact events.
		/// </summary>
		/// <param name="item"></param>
		private void Subscribe(IContact item)
		{
			IContactWithOnlineState contact = item as IContactWithOnlineState;
			if (contact != null)
				contact.OnOnlineStateChanged += ContactOnOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the contact events.
		/// </summary>
		/// <param name="item"></param>
		private void Unsubscribe(IContact item)
		{
			IContactWithOnlineState contact = item as IContactWithOnlineState;
			if (contact != null)
				contact.OnOnlineStateChanged -= ContactOnOnlineStateChanged;
		}

		/// <summary>
		/// Called when the contact online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContactOnOnlineStateChanged(object sender, OnlineStateEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWtcReferencedContactView view)
		{
			base.Subscribe(view);

			view.OnContactPressed += ViewOnContactPressed;
			view.OnFavoriteButtonPressed += ViewOnFavoriteButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcReferencedContactView view)
		{
			base.Unsubscribe(view);

			view.OnContactPressed -= ViewOnContactPressed;
			view.OnFavoriteButtonPressed -= ViewOnFavoriteButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the contact.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnContactPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the favorite button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Contact == null)
				return;

			ToggleFavorite();
		}

		#endregion
	}
}