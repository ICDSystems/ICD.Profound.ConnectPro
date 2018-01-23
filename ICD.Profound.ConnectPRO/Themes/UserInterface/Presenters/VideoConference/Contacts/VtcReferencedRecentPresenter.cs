using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.ConferenceSources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcReferencedRecentPresenter : AbstractVtcReferencedContactsPresenterBase, IVtcReferencedRecentPresenter
	{
		private IConferenceSource m_Recent;

		public IConferenceSource Recent
		{
			get { return m_Recent; }
			set
			{
				if (value == m_Recent)
					return;

				m_Recent = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedRecentPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Override to control the display name for this contact.
		/// </summary>
		/// <returns></returns>
		protected override string GetName()
		{
			return Recent == null
				       ? null
				       : string.IsNullOrEmpty(Recent.Name)
					         ? Recent.Number
					         : Recent.Name;
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
			Recent = model as IConferenceSource;
		}

		protected override void Dial()
		{
			IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
			if (manager != null && m_Recent != null)
				manager.Dial(m_Recent);
		}
	}
}
