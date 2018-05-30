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
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

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

		private readonly IVtcKeyboardPresenter m_Keyboard;

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
				Selected = null;

				Refresh();
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

				Refresh();
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

			m_Keyboard = nav.LazyLoadPresenter<IVtcKeyboardPresenter>();
			m_Keyboard.OnExitButtonPressed += KeyboardOnExitButtonPressed;
		}

		private void KeyboardOnExitButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Keyboard.ShowView(false);
			ShowView(true);
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

				view.SetDirectoryButtonSelected(m_DirectoryMode == eDirectoryMode.Contacts);
				view.SetFavoritesButtonSelected(m_DirectoryMode == eDirectoryMode.Favorites);
				view.SetRecentButtonSelected(m_DirectoryMode == eDirectoryMode.Recents);

				view.SetCallButtonEnabled(callEnabled);
				view.SetHangupButtonEnabled(hangupEnabled);
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
											 .OrderBy(c => c is IContact)
											 .ThenBy(c => c.Name)
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
								.OrderBy(f => f.Name)
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

			IDialingDeviceControl videoDialer = room.ConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			CiscoCodec codec = videoDialer == null ? null : videoDialer.Parent as CiscoCodec;
			if (codec == null)
				return;

			DirectoryComponent component = codec.Components.GetComponent<DirectoryComponent>();
			m_DirectoryBrowser = new DirectoryBrowser(component);

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

			view.OnCallButtonPressed += ViewOnCallButtonPressed;
			view.OnDirectoryButtonPressed += ViewOnDirectoryButtonPressed;
			view.OnFavoritesButtonPressed += ViewOnFavoritesButtonPressed;
			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed += ViewOnRecentButtonPressed;
			view.OnBackButtonPressed += ViewOnBackButtonPressed;
			view.OnHomeButtonPressed += ViewOnHomeButtonPressed;
			view.OnSearchButtonPressed += ViewOnSearchButtonPressed;
			view.OnManualDialButtonPressed += ViewOnManualDialButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcContactsView view)
		{
			base.Unsubscribe(view);

			view.OnCallButtonPressed -= ViewOnCallButtonPressed;
			view.OnDirectoryButtonPressed -= ViewOnDirectoryButtonPressed;
			view.OnFavoritesButtonPressed -= ViewOnFavoritesButtonPressed;
			view.OnHangupButtonPressed -= ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed -= ViewOnRecentButtonPressed;
			view.OnBackButtonPressed -= ViewOnBackButtonPressed;
			view.OnHomeButtonPressed -= ViewOnHomeButtonPressed;
			view.OnSearchButtonPressed -= ViewOnSearchButtonPressed;
			view.OnManualDialButtonPressed -= ViewOnManualDialButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the manual dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnManualDialButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Keyboard.ShowView(true);
		}

		/// <summary>
		/// Called when the user presses the search button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnSearchButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.NavigateTo<IDisabledAlertPresenter>();
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnHomeButtonPressed(object sender, EventArgs eventArgs)
		{
			m_DirectoryBrowser.GoToRoot();
		}

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnBackButtonPressed(object sender, EventArgs eventArgs)
		{
			m_DirectoryBrowser.GoUp();
		}

		/// <summary>
		/// Called when the user presses the recents button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnRecentButtonPressed(object sender, EventArgs eventArgs)
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
		private void ViewOnDirectoryButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Contacts;
		}

		/// <summary>
		/// Called when the user presses the call button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCallButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_Selected == null)
				return;

			IVtcReferencedContactsPresenterBase presenter = m_Selected;
			Selected = null;

			presenter.Dial();
		}

		/// <summary>
		/// Called when the user presses the hangup button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_SubscribedConferenceManager == null || m_SubscribedConferenceManager.IsInCall == eInCall.None)
				return;

			// Go to the list of active calls
			Navigation.LazyLoadPresenter<IVtcButtonListPresenter>().ShowMenu(VtcButtonListPresenter.INDEX_ACTIVE_CALLS);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			// Clear the selection
			Selected = null;
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
