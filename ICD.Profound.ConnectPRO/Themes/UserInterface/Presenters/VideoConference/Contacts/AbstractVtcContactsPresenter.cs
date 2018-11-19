using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.Directory;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public abstract class AbstractVtcContactsPresenter<TView> : AbstractUiPresenter<TView>, IVtcContactsPresenter<TView>
		where TView : class, IVtcContactsView
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly VtcReferencedContactsPresenterFactory m_ContactsFactory;
		private readonly DirectoryControlBrowser m_DirectoryBrowser;

		private	IVtcReferencedContactsPresenterBase m_Selected;

		private IConferenceManager m_SubscribedConferenceManager;

		#region Properties

		protected IVtcReferencedContactsPresenterBase Selected
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

		protected IConferenceManager ConferenceManager { get { return m_SubscribedConferenceManager; } }

		protected DirectoryControlBrowser DirectoryBrowser { get { return m_DirectoryBrowser; } }

		protected virtual bool CallButtonEnabled { get { return m_Selected != null; } }

		protected virtual bool HangupButtonEnabled
		{
			get { return m_SubscribedConferenceManager != null && m_SubscribedConferenceManager.ActiveConference != null; }
		}

		protected virtual bool HideFavoriteIcons { get { return false; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractVtcContactsPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ContactsFactory = new VtcReferencedContactsPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);

			m_DirectoryBrowser = new DirectoryControlBrowser();
			Subscribe(m_DirectoryBrowser);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(DirectoryBrowser);
			UnsubscribeChildren();

			m_ContactsFactory.Dispose();
			DirectoryBrowser.Dispose();
		}

		/// <summary>
		/// Refresh the visual state of the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(TView view)
		{
			base.Refresh(view);

			IDirectoryFolder current = DirectoryBrowser == null ? null : DirectoryBrowser.GetCurrentFolder();
			if (current != null && current.ChildCount == 0)
				DirectoryBrowser.PopulateCurrentFolder();

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<ModelPresenterTypeInfo> contacts = GetContacts();

				foreach (IVtcReferencedContactsPresenterBase presenter in m_ContactsFactory.BuildChildren(contacts))
				{
					presenter.Selected = presenter == m_Selected;
					presenter.HideFavoriteIcon = HideFavoriteIcons;
					presenter.ShowView(true);
				}

				view.SetCallButtonEnabled(CallButtonEnabled);
				view.SetHangupButtonEnabled(HangupButtonEnabled);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private IEnumerable<IVtcReferencedContactsView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		protected abstract IEnumerable<ModelPresenterTypeInfo> GetContacts();

		#endregion

		#region Directory Browser Callbacks

		/// <summary>
		/// Subscribe to the browser events.
		/// </summary>
		/// <param name="browser"></param>
		private void Subscribe(DirectoryControlBrowser browser)
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
		private void Unsubscribe(DirectoryControlBrowser browser)
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
		private void BrowserOnPathChanged(object sender, DirectoryFolderEventArgs folderEventArgs)
		{
			m_Selected = null;

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

			IConferenceDeviceControl dialer = m_SubscribedConferenceManager.GetDialingProvider(eCallType.Video);
			IDeviceBase parent = dialer == null ? null : dialer.Parent;
			IDirectoryControl directory = parent == null ? null : parent.Controls.GetControl<IDirectoryControl>();
			
			DirectoryBrowser.SetControl(directory);
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			DirectoryBrowser.SetControl(null);

			if (m_SubscribedConferenceManager == null)
				return;

			m_SubscribedConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;

			m_SubscribedConferenceManager = null;
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

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(TView view)
		{
			base.Subscribe(view);

			view.OnCallButtonPressed += ViewOnCallButtonPressed;
			view.OnDirectoryButtonPressed += ViewOnDirectoryButtonPressed;
			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed += ViewOnRecentButtonPressed;
			view.OnBackButtonPressed += ViewOnBackButtonPressed;
			view.OnHomeButtonPressed += ViewOnHomeButtonPressed;
			view.OnManualDialButtonPressed += ViewOnManualDialButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(TView view)
		{
			base.Unsubscribe(view);

			view.OnCallButtonPressed -= ViewOnCallButtonPressed;
			view.OnDirectoryButtonPressed -= ViewOnDirectoryButtonPressed;
			view.OnHangupButtonPressed -= ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed -= ViewOnRecentButtonPressed;
			view.OnBackButtonPressed -= ViewOnBackButtonPressed;
			view.OnHomeButtonPressed -= ViewOnHomeButtonPressed;
			view.OnManualDialButtonPressed -= ViewOnManualDialButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the manual dial button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void ViewOnManualDialButtonPressed(object sender, EventArgs eventArgs)
		{
			Navigation.LazyLoadPresenter<IVtcKeyboardPresenter>().ShowView(KeyboardDialCallback);
		}

		/// <summary>
		/// Called when the user presses dial on the keyboard/keypad.
		/// </summary>
		/// <param name="number"></param>
		private void KeyboardDialCallback(string number)
		{
			IConferenceDeviceControl control = Room == null ? null : Room.ConferenceManager.GetDialingProvider(eCallType.Video);
			if (control != null)
				control.Dial(new SipDialContext { DialString = number, CallType = eCallType.Video });

			ShowView(true);
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void ViewOnHomeButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryBrowser.GoToRoot();
		}

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void ViewOnBackButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryBrowser.GoUp();
		}

		/// <summary>
		/// Called when the user presses the recents button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void ViewOnRecentButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		/// <summary>
		/// Called when the user presses the directory button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void ViewOnDirectoryButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		/// <summary>
		/// Called when the user presses the call button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void ViewOnCallButtonPressed(object sender, EventArgs eventArgs)
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
		protected virtual void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_SubscribedConferenceManager == null || m_SubscribedConferenceManager.IsInCall == eInCall.None)
				return;

			// Go to the list of active calls
			Navigation.NavigateTo<IVtcButtonListPresenter>().ShowMenu(VtcButtonListPresenter.INDEX_ACTIVE_CALLS);
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

			// Return to root
			DirectoryBrowser.GoToRoot();
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
				if (DirectoryBrowser != null)
					DirectoryBrowser.EnterFolder(folderPresenter.Folder);
			}
		}

		#endregion
	}
}
