using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Cisco;
using ICD.Connect.Conferencing.Cisco.Components.Directory;
using ICD.Connect.Conferencing.Cisco.Components.Directory.Tree;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Hangup;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcContactsPresenter : AbstractPresenter<IVtcContactsView>, IVtcContactsPresenter
	{
		private enum eDirectoryMode
		{
			Contacts,
			Favorites,
			Recents
		}

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly VtcReferencedContactsPresenterFactory m_ContactsFactory;

		private eDirectoryMode m_DirectoryMode;
		private DirectoryBrowser m_DirectoryBrowser;
		private	IVtcReferencedContactsPresenterBase m_Selected;
		private IConferenceManager m_SubscribedConferenceManager;

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

				RefreshIfVisible();
			}
		}

		private IVtcReferencedContactsPresenterBase Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcContactsPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ContactsFactory = new VtcReferencedContactsPresenterFactory(nav, ItemFactory);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			if (m_DirectoryBrowser != null)
			{
				Unsubscribe(m_DirectoryBrowser);
				m_DirectoryBrowser.Dispose();
			}

			UnsubscribeChildren();
			m_ContactsFactory.Dispose();
		}

		private IEnumerable<IVtcReferencedContactsView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		/// <summary>
		/// Refresh the visual state of the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcContactsView view)
		{
			base.Refresh(view);

			if (m_DirectoryBrowser != null)
				m_DirectoryBrowser.PopulateCurrentFolder();

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeChildren();

				IEnumerable<ModelPresenterTypeInfo> contacts = GetContacts();
				foreach (IVtcReferencedContactsPresenterBase presenter in m_ContactsFactory.BuildChildren(contacts))
				{
					Subscribe(presenter);

					presenter.Selected = presenter == m_Selected;
					presenter.ShowView(true);
				}

				bool callEnabled = m_Selected != null;

				IConference active = m_SubscribedConferenceManager == null ? null : m_SubscribedConferenceManager.ActiveConference;
				bool hangupEnabled = active != null;

				string searchText = string.Empty;

				view.SetDirectoryButtonSelected(m_DirectoryMode == eDirectoryMode.Contacts);
				view.SetFavoritesButtonSelected(m_DirectoryMode == eDirectoryMode.Favorites);
				view.SetRecentButtonSelected(m_DirectoryMode == eDirectoryMode.Recents);

				view.SetCallButtonEnabled(callEnabled);
				view.SetHangupButtonEnabled(hangupEnabled);

				view.SetSearchBarText(searchText);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<ModelPresenterTypeInfo> GetContacts()
		{
			return GetContacts(m_DirectoryMode);
		}

		private IEnumerable<ModelPresenterTypeInfo> GetContacts(eDirectoryMode directoryMode)
		{
			switch (directoryMode)
			{
				case eDirectoryMode.Contacts:
					if (m_DirectoryBrowser == null)
						return Enumerable.Empty<ModelPresenterTypeInfo>();

					return m_DirectoryBrowser.GetCurrentFolder()
					                         .GetChildren()
					                         .Select(c =>
					                                 {
						                                 ModelPresenterTypeInfo.ePresenterType type = (c is IFolder)
							                                                                              ? ModelPresenterTypeInfo
								                                                                                .ePresenterType.Folder
							                                                                              : ModelPresenterTypeInfo
								                                                                                .ePresenterType.Contact;
						                                 return new ModelPresenterTypeInfo(type, c);
					                                 });
				
				case eDirectoryMode.Favorites:
					return
						m_SubscribedConferenceManager == null || m_SubscribedConferenceManager.Favorites == null
							? Enumerable.Empty<ModelPresenterTypeInfo>()
							: m_SubscribedConferenceManager.Favorites
								.GetFavorites()
								.Select(f => new ModelPresenterTypeInfo(ModelPresenterTypeInfo.ePresenterType.Favorite, f));
				
				case eDirectoryMode.Recents:
					return
						m_SubscribedConferenceManager == null
							? Enumerable.Empty<ModelPresenterTypeInfo>()
							: m_SubscribedConferenceManager
								.GetRecentSources()
								.Reverse()
								.Distinct()
								.Select(c => new ModelPresenterTypeInfo(ModelPresenterTypeInfo.ePresenterType.Recent, c));

				default:
					throw new ArgumentOutOfRangeException("directoryMode");
			}
		}

		public override void SetRoom(IConnectProRoom room)
		{
			Unsubscribe(m_DirectoryBrowser);

			if (m_DirectoryBrowser != null)
				m_DirectoryBrowser.Dispose();
			m_DirectoryBrowser = null;

			base.SetRoom(room);

			if (room == null)
				return;

			CiscoCodec codec = room.Originators.GetInstanceRecursive<CiscoCodec>();
			if (codec == null)
				return;

			DirectoryComponent component = codec.Components.GetComponent<DirectoryComponent>();
			m_DirectoryBrowser = new DirectoryBrowser(component)
			{
				PhonebookType = ePhonebookType.Corporate
			};

			Subscribe(m_DirectoryBrowser);

			// Show the existing directory contents
			Refresh();
		}

		#region Directory Browser Callbacks

		/// <summary>
		/// Subscribe to the browser events.
		/// </summary>
		/// <param name="browser"></param>
		private void Subscribe(DirectoryBrowser browser)
		{
			if (browser == null)
				return;

			browser.OnPathChanged += BrowserOnPathChanged;
			browser.OnPathContentsChanged += BrowserOnPathContentsChanged;
		}

		/// <summary>
		/// Unsubscribe from the browser events.
		/// </summary>
		/// <param name="browser"></param>
		private void Unsubscribe(DirectoryBrowser browser)
		{
			if (browser == null)
				return;

			browser.OnPathChanged -= BrowserOnPathChanged;
			browser.OnPathContentsChanged -= BrowserOnPathContentsChanged;
		}

		/// <summary>
		/// Called when the current folder contents change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void BrowserOnPathContentsChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the browser path changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="folderEventArgs"></param>
		private void BrowserOnPathChanged(object sender, FolderEventArgs folderEventArgs)
		{
			m_Selected = null;

			if (m_DirectoryBrowser != null)
				m_DirectoryBrowser.PopulateCurrentFolder();

			RefreshIfVisible();
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			m_SubscribedConferenceManager = room.ConferenceManager;
			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged += ConferenceManagerOnActiveSourceStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged -= ConferenceManagerOnActiveSourceStatusChanged;
		}

		/// <summary>
		/// Called when we enter/leave a call.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ConferenceManagerOnInCallChanged(object sender, InCallEventArgs args)
		{
			RefreshIfVisible();
		}

		private void ConferenceManagerOnActiveSourceStatusChanged(object sender, ConferenceSourceStatusEventArgs args)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcContactsView view)
		{
			base.Subscribe(view);

			view.OnTextEntered += ViewOnTextEntered;
			view.OnCallButtonPressed += ViewOnCallButtonPressed;
			view.OnDirectoryButtonPressed += ViewOnDirectoryButtonPressed;
			view.OnFavoritesButtonPressed += ViewOnFavoritesButtonPressed;
			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed += ViewOnRecentButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcContactsView view)
		{
			base.Unsubscribe(view);

			view.OnTextEntered -= ViewOnTextEntered;
			view.OnCallButtonPressed -= ViewOnCallButtonPressed;
			view.OnDirectoryButtonPressed -= ViewOnDirectoryButtonPressed;
			view.OnFavoritesButtonPressed -= ViewOnFavoritesButtonPressed;
			view.OnHangupButtonPressed -= ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed -= ViewOnRecentButtonPressed;
		}

		private void ViewOnRecentButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Recents;
		}

		private void ViewOnFavoritesButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Favorites;
		}

		private void ViewOnDirectoryButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Contacts;
		}

		private void ViewOnCallButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Selected == null)
				return;

			m_Selected.Dial();
		}

		private void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			IVtcHangupPresenter presenter = Navigation.LazyLoadPresenter<IVtcHangupPresenter>();
			int sourceCount = presenter.GetSources().Count();

			// Don't do anything if there are no sources.
			if (sourceCount == 0)
				return;

			// If there is only one source hangup immediately, otherwise provide a menu
			if (sourceCount == 1)
				presenter.HangupAll();
			else
				presenter.ShowView(true);
		}

		private void ViewOnTextEntered(object sender, StringEventArgs stringEventArgs)
		{
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IVtcReferencedContactsPresenterBase presenter in m_ContactsFactory)
				Unsubscribe(presenter);
		}

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IVtcReferencedContactsPresenterBase child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IVtcReferencedContactsPresenterBase child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnPressed;
		}

		/// <summary>
		/// Called when the user presses the contact presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnPressed(object sender, EventArgs eventArgs)
		{
			IVtcReferencedContactsPresenterBase presenter = sender as IVtcReferencedContactsPresenterBase;
			if (presenter == null)
				return;

			// Edge case - when we press a folder we want to enter it, not select it
			IVtcReferencedFolderPresenter folderPresenter = sender as IVtcReferencedFolderPresenter;

			if (folderPresenter == null)
			{
				Selected = presenter;
			}
			else
			{
				if (m_DirectoryBrowser != null)
					m_DirectoryBrowser.EnterFolder(folderPresenter.Folder);
			}
		}

		#endregion
	}
}
