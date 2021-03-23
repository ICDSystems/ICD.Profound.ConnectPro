using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Directory;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference.Contacts
{
	[PresenterBinding(typeof(IReferencedContactPresenter))]
	public sealed class ReferencedContactPresenter : AbstractTouchDisplayComponentPresenter<IReferencedContactView>, IReferencedContactPresenter
	{
		/// <summary>
		/// Raised when the user presses the contact.
		/// </summary>
		public event EventHandler OnPressed;

		/// <summary>
		/// Raised when the online status changes.
		/// </summary>
		public event EventHandler<OnlineStateEventArgs> OnOnlineStateChanged;

		/// <summary>
		/// Raised when the favorite button is pressed.
		/// </summary>
		public event EventHandler OnFavoriteButtonPressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private IContact m_Contact;
		private bool m_IsFavorite;
		private eOnlineState m_OnlineState;

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

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the IsFavorite state.
		/// </summary>
		public bool IsFavorite
		{
			get { return m_IsFavorite; }
			set
			{
				if (value == m_IsFavorite)
					return;

				m_IsFavorite = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets the online state.
		/// </summary>
		public eOnlineState OnlineState
		{
			get { return m_OnlineState; }
			private set
			{
				if (value == m_OnlineState)
					return;

				m_OnlineState = value;

				OnOnlineStateChanged.Raise(this, new OnlineStateEventArgs(m_OnlineState));

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
		public ReferencedContactPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
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
			OnOnlineStateChanged = null;
			OnFavoriteButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedContactView view)
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
				view.SetAvatarImageVisibility(true);
				view.SetAvatarImagePath(zoomContact == null || string.IsNullOrEmpty(zoomContact.AvatarUrl) 
					? "ic_zoom_participants_head"
					: zoomContact.AvatarUrl);

				// Favorite state
				view.SetFavoriteButtonSelected(IsFavorite);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
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
			OnlineState = e.Data;
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IReferencedContactView view)
		{
			base.Subscribe(view);

			view.OnContactPressed += ViewOnContactPressed;
			view.OnFavoriteButtonPressed += ViewOnFavoriteButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedContactView view)
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
			OnFavoriteButtonPressed.Raise(this);
		}

		#endregion
	}
}