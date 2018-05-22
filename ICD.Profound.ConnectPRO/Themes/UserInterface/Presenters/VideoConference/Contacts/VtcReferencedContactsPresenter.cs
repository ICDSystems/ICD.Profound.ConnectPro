using System;
using System.Linq;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Favorites;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcReferencedContactsPresenter : AbstractVtcReferencedContactsPresenterBase,
	                                                     IVtcReferencedContactsPresenter
	{
		private IContact m_Contact;

		public IContact Contact
		{
			get { return m_Contact; }
			set
			{
				if (value == m_Contact)
					return;

				m_Contact = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedContactsPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Override to control the display name for this contact.
		/// </summary>
		/// <returns></returns>
		protected override string GetName()
		{
			return Contact == null ? null : Contact.Name;
		}

		/// <summary>
		/// Override to control the favorite state for this contact.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavorite()
		{
			if (m_Contact == null)
				return false;

			IFavorites favorites = Favorites;
			if (favorites == null)
				return false;

			return favorites.GetFavorite(m_Contact) != null;
		}

		protected override void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
			base.ViewOnFavoriteButtonPressed(sender, eventArgs);

			if (m_Contact == null)
				return;

			IFavorites favorites = Favorites;
			if (favorites == null)
				return;

			if (GetIsFavorite())
				favorites.RemoveFavorite(m_Contact);
			else
				favorites.SubmitFavorite(m_Contact);

			RefreshIfVisible();
		}

		/// <summary>
		/// Override to control the visibility of the favorite button for this contact.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavoriteVisible()
		{
			return m_Contact != null;
		}

		protected override void SetModel(object model)
		{
			Contact = model as IContact;
		}

		protected override void Dial()
		{
			IDialingDeviceControl dialer = Room == null ? null : Room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			if (dialer != null && m_Contact != null)
				dialer.Dial(m_Contact.GetContactMethods().First().Number);
		}
	}
}
