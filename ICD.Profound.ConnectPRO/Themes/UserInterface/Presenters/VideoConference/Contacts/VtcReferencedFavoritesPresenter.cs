using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Favorites;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	[PresenterBinding(typeof(IVtcReferencedFavoritesPresenter))]
	public sealed class VtcReferencedFavoritesPresenter : AbstractVtcReferencedContactsPresenterBase,
	                                                      IVtcReferencedFavoritesPresenter
	{
		private Favorite m_Favorite;

		[CanBeNull]
		public Favorite Favorite
		{
			get { return m_Favorite; }
			set
			{
				if (value == m_Favorite)
					return;

				m_Favorite = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedFavoritesPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Override to control the display name for this contact.
		/// </summary>
		/// <returns></returns>
		protected override string GetName()
		{
			return Favorite == null ? null : Favorite.Name;
		}

		/// <summary>
		/// Override to control the favorite state for this contact.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavorite()
		{
			return true;
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
			Favorite = model as Favorite;
		}

		protected override void Dial()
		{
			var dialer = ActiveConferenceControl;
			if (dialer != null && m_Favorite != null)
				dialer.Dial(m_Favorite.GetDialContexts().First());
		}

		protected override void ViewOnFavoriteButtonPressed(object sender, EventArgs eventArgs)
		{
			base.ViewOnFavoriteButtonPressed(sender, eventArgs);

			if (m_Favorite == null || Room == null)
				return;

			Favorite.Delete(Room.Id, m_Favorite);

			Navigation.LazyLoadPresenter<IVtcContactsNormalPresenter>().RefreshIfVisible();
			Navigation.LazyLoadPresenter<IVtcContactsPolycomPresenter>().RefreshIfVisible();
		}
	}
}
