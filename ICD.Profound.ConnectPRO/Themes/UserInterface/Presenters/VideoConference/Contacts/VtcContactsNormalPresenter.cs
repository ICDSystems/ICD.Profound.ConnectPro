using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	[PresenterBinding(typeof(IVtcContactsNormalPresenter))]
	public sealed class VtcContactsNormalPresenter : AbstractVtcContactsPresenter<IVtcContactsNormalView>, IVtcContactsNormalPresenter
	{
		private enum eDirectoryMode
		{
			Contacts,
			Favorites,
			Recents
		}

		private readonly SafeCriticalSection m_RefreshSection;

		private eDirectoryMode m_DirectoryMode;

		#region Properties

		/// <summary>
		/// Gets/sets the directory mode for populating the contacts list.
		/// </summary>
		private eDirectoryMode DirectoryMode
		{
			get { return m_DirectoryMode; }
			set
			{
				if (value == m_DirectoryMode)
					return;

				m_DirectoryMode = value;
				Selected = null;

				Refresh();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcContactsNormalPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Refresh the visual state of the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcContactsNormalView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetDirectoryButtonSelected(m_DirectoryMode == eDirectoryMode.Contacts);
				view.SetFavoritesButtonSelected(m_DirectoryMode == eDirectoryMode.Favorites);
				view.SetRecentButtonSelected(m_DirectoryMode == eDirectoryMode.Recents);
				view.SetNavigationButtonsVisible(m_DirectoryMode == eDirectoryMode.Contacts);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		protected override IEnumerable<ModelPresenterTypeInfo> GetContacts()
		{
			return GetContacts(m_DirectoryMode);
		}

		private IEnumerable<ModelPresenterTypeInfo> GetContacts(eDirectoryMode directoryMode)
		{
			switch (directoryMode)
			{
				case eDirectoryMode.Contacts:
					if (DirectoryBrowser == null)
						return Enumerable.Empty<ModelPresenterTypeInfo>();

					IDirectoryFolder current = DirectoryBrowser.GetCurrentFolder();
					if (current == null)
						return Enumerable.Empty<ModelPresenterTypeInfo>();

					return current
						.GetFolders()
						.Cast<object>()
						.Concat(current.GetContacts())
						.OrderBy(c => c is IContact)
						.ThenBy(c =>
						        {
							        if (c is IContact)
								        return (c as IContact).Name;
									if (c is IDirectoryFolder)
										return (c as IDirectoryFolder).Name;

									// This should never happen
									throw new InvalidOperationException();
						        })
						.Select(c =>
						        {
							        ModelPresenterTypeInfo.ePresenterType type = (c is IDirectoryFolder)
								                                                     ? ModelPresenterTypeInfo
									                                                       .ePresenterType.Folder
								                                                     : ModelPresenterTypeInfo
									                                                       .ePresenterType.Contact;
							        return new ModelPresenterTypeInfo(type, c);
						        });
				
				case eDirectoryMode.Favorites:
					return
						ConferenceManager == null || ConferenceManager.Favorites == null
							? Enumerable.Empty<ModelPresenterTypeInfo>()
							: ConferenceManager.Favorites
								.GetFavorites()
								.OrderBy(f => f.Name)
								.Select(f => new ModelPresenterTypeInfo(ModelPresenterTypeInfo.ePresenterType.Favorite, f));
				
				case eDirectoryMode.Recents:
					return
						ConferenceManager == null
							? Enumerable.Empty<ModelPresenterTypeInfo>()
							: ConferenceManager
								.GetRecentParticipants()
								.Reverse()
								.Distinct()
								.Select(c => new ModelPresenterTypeInfo(ModelPresenterTypeInfo.ePresenterType.Recent, c));

				default:
					throw new ArgumentOutOfRangeException("directoryMode");
			}
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcContactsNormalView view)
		{
			base.Subscribe(view);

			view.OnFavoritesButtonPressed += ViewOnFavoritesButtonPressed;
			view.OnSearchButtonPressed += ViewOnSearchButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcContactsNormalView view)
		{
			base.Unsubscribe(view);

			view.OnFavoritesButtonPressed -= ViewOnFavoritesButtonPressed;
			view.OnSearchButtonPressed -= ViewOnSearchButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the search button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSearchButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show("This function is currently disabled.", 2 * 1000, GenericAlertPresenterButton.Dismiss);
		}

		/// <summary>
		/// Called when the user presses the recents button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnRecentButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Recents;
		}

		/// <summary>
		/// Called when the user presses the favourites button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnFavoritesButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Favorites;
		}

		/// <summary>
		/// Called when the user presses the directory button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void ViewOnDirectoryButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Contacts;
		}

		#endregion
	}
}
