using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Contacts;
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
			// todo
			return false;
		}

		/// <summary>
		/// Override to control the visibility of the favorite button for this contact.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavoriteVisible()
		{
			return true;
		}

		protected override void SetModel(object model)
		{
			Contact = model as IContact;
		}

		protected override void Dial()
		{
			IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
			if (manager != null && m_Contact != null)
				manager.Dial(m_Contact);
		}
	}
}
