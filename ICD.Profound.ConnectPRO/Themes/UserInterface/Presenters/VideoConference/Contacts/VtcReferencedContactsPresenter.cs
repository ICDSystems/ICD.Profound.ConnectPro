using System;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Favorites;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	[PresenterBinding(typeof(IVtcReferencedContactsPresenter))]
	public sealed class VtcReferencedContactsPresenter : AbstractVtcReferencedContactsPresenterBase,
	                                                     IVtcReferencedContactsPresenter
	{
		private IContact m_Contact;

		[CanBeNull]
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
		public VtcReferencedContactsPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
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
			return m_Contact != null &&
			       Room != null &&
			       Favorite.Contains(Room.Id, m_Contact);
		}

		protected override void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
			base.ViewOnFavoriteButtonPressed(sender, eventArgs);

			if (m_Contact == null || Room == null)
				return;

			Favorite.Toggle(Room.Id, m_Contact);

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

			var dialer = ActiveConferenceControl;
			if (dialer != null && m_Contact != null)
				dialer.Dial(m_Contact);
		}
	}
}
