using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Conferencing.Directory;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Polycom.Devices.Codec;
using ICD.Connect.Conferencing.Polycom.Devices.Codec.Components.Button;
using ICD.Connect.Devices;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcContactsPolycomPresenter : AbstractVtcContactsPresenter<IVtcContactsPolycomView>, IVtcContactsPolycomPresenter
	{
		private enum ePolycomDirectoryMode
		{
			Navigation,
			Local,
			Recents
		}

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly VtcReferencedContactsPresenterFactory m_ContactsFactory;
		private readonly DirectoryControlBrowser m_DirectoryBrowser;

		private readonly IVtcKeyboardPresenter m_Keyboard;
		private readonly IVtcKeypadPresenter m_Keypad;

		private ePolycomDirectoryMode m_DirectoryMode;
		private	IVtcReferencedContactsPresenterBase m_Selected;

		private IConferenceManager m_SubscribedConferenceManager;

		private ButtonComponent m_ButtonComponent;

		#region Properties

		/// <summary>
		/// Gets/sets the directory mode for populating the contacts list.
		/// </summary>
		private ePolycomDirectoryMode DirectoryMode
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

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcContactsPolycomPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ContactsFactory = new VtcReferencedContactsPresenterFactory(nav, ItemFactory);

			m_Keyboard = nav.LazyLoadPresenter<IVtcKeyboardPresenter>();
			m_Keyboard.OnKeypadButtonPressed += KeyboardOnKeypadButtonPressed;
			m_Keyboard.OnDialButtonPressed += KeyboardOnDialButtonPressed;

			m_Keypad = nav.LazyLoadPresenter<IVtcKeypadPresenter>();
			m_Keypad.OnKeyboardButtonPressed += KeypadOnKeyboardButtonPressed;
			m_Keypad.OnDialButtonPressed += KeypadOnDialButtonPressed;

			m_DirectoryBrowser = new DirectoryControlBrowser();
			Subscribe(m_DirectoryBrowser);
		}

		private void KeypadOnDialButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Keypad.ShowView(true);

			ShowView(true);
		}

		private void KeyboardOnDialButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Keypad.ShowView(false);

			ShowView(true);
		}

		private void KeyboardOnKeypadButtonPressed(object sender, EventArgs eventArgs)
		{
			m_Keyboard.ShowView(false);
			m_Keypad.ShowView(true);
		}

		private void KeypadOnKeyboardButtonPressed(object sender, EventArgs e)
		{
			m_Keypad.ShowView(false);
			m_Keyboard.ShowView(true);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_DirectoryBrowser);
			UnsubscribeChildren();

			m_ContactsFactory.Dispose();
			m_DirectoryBrowser.Dispose();
		}

		/// <summary>
		/// Refresh the visual state of the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcContactsPolycomView view)
		{
			base.Refresh(view);

			IDirectoryFolder current = m_DirectoryBrowser == null ? null : m_DirectoryBrowser.GetCurrentFolder();
			if (current != null && current.ChildCount == 0)
				m_DirectoryBrowser.PopulateCurrentFolder();

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<ModelPresenterTypeInfo> contacts = GetContacts();

				foreach (IVtcReferencedContactsPresenterBase presenter in m_ContactsFactory.BuildChildren(contacts, Subscribe, Unsubscribe))
				{
					presenter.Selected = presenter == m_Selected;
					presenter.HideFavoriteIcon = true;
					presenter.ShowView(true);
				}

				bool callEnabled = m_DirectoryMode == ePolycomDirectoryMode.Navigation || m_Selected != null;

				IConference active = m_SubscribedConferenceManager == null ? null : m_SubscribedConferenceManager.ActiveConference;
				bool hangupEnabled = m_DirectoryMode == ePolycomDirectoryMode.Navigation || active != null;

				view.SetDPadVisible(m_DirectoryMode == ePolycomDirectoryMode.Navigation);

				view.SetDPadButtonSelected(m_DirectoryMode == ePolycomDirectoryMode.Navigation);
				view.SetLocalButtonSelected(m_DirectoryMode == ePolycomDirectoryMode.Local);
				view.SetRecentButtonSelected(m_DirectoryMode == ePolycomDirectoryMode.Recents);

				view.SetBackButtonVisible(m_DirectoryMode == ePolycomDirectoryMode.Local ||
				                          m_DirectoryMode == ePolycomDirectoryMode.Navigation);
				view.SetHomeButtonVisible(m_DirectoryMode == ePolycomDirectoryMode.Local ||
				                          m_DirectoryMode == ePolycomDirectoryMode.Navigation);
				view.SetDirectoryButtonVisible(m_DirectoryMode == ePolycomDirectoryMode.Navigation);

				view.SetCallButtonEnabled(callEnabled);
				view.SetHangupButtonEnabled(hangupEnabled);
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

		private IEnumerable<ModelPresenterTypeInfo> GetContacts()
		{
			return GetContacts(m_DirectoryMode);
		}

		private IEnumerable<ModelPresenterTypeInfo> GetContacts(ePolycomDirectoryMode directoryMode)
		{
			switch (directoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					return Enumerable.Empty<ModelPresenterTypeInfo>();

				case ePolycomDirectoryMode.Local:
					if (m_DirectoryBrowser == null)
						return Enumerable.Empty<ModelPresenterTypeInfo>();

					IDirectoryFolder current = m_DirectoryBrowser.GetCurrentFolder();
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
				
				case ePolycomDirectoryMode.Recents:
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

			IDialingDeviceControl videoDialer = m_SubscribedConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			PolycomGroupSeriesDevice videoDialerDevice = videoDialer == null ? null : videoDialer.Parent as PolycomGroupSeriesDevice;
			m_ButtonComponent = videoDialerDevice == null ? null : videoDialerDevice.Components.GetComponent<ButtonComponent>();

			m_SubscribedConferenceManager.OnInCallChanged += ConferenceManagerOnInCallChanged;
			m_SubscribedConferenceManager.OnActiveSourceStatusChanged += ConferenceManagerOnActiveSourceStatusChanged;

			IDialingDeviceControl dialer = m_SubscribedConferenceManager.GetDialingProvider(eConferenceSourceType.Video);
			IDeviceBase parent = dialer == null ? null : dialer.Parent;
			IDirectoryControl directory = parent == null ? null : parent.Controls.GetControl<IDirectoryControl>();
			
			m_DirectoryBrowser.SetControl(directory);
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			m_DirectoryBrowser.SetControl(null);

			if (m_SubscribedConferenceManager != null)
			{
				m_SubscribedConferenceManager.OnInCallChanged -= ConferenceManagerOnInCallChanged;
				m_SubscribedConferenceManager.OnActiveSourceStatusChanged -= ConferenceManagerOnActiveSourceStatusChanged;
			}

			m_ButtonComponent = null;

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
		protected override void Subscribe(IVtcContactsPolycomView view)
		{
			base.Subscribe(view);

			view.OnCallButtonPressed += ViewOnCallButtonPressed;
			view.OnNavigationButtonPressed += ViewOnNavigationButtonPressed;
			view.OnLocalButtonPressed += ViewOnLocalButtonPressed;
			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed += ViewOnRecentButtonPressed;
			view.OnBackButtonPressed += ViewOnBackButtonPressed;
			view.OnHomeButtonPressed += ViewOnHomeButtonPressed;
			view.OnDirectoryButtonPressed += ViewOnDirectoryButtonPressed;
			view.OnManualDialButtonPressed += ViewOnManualDialButtonPressed;

			view.OnDPadDownButtonPressed += ViewOnDPadDownButtonPressed;
			view.OnDPadLeftButtonPressed += ViewOnDPadLeftButtonPressed;
			view.OnDPadRightButtonPressed += ViewOnDPadRightButtonPressed;
			view.OnDPadUpButtonPressed += ViewOnDPadUpButtonPressed;
			view.OnDPadSelectButtonPressed += ViewOnDPadSelectButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcContactsPolycomView view)
		{
			base.Unsubscribe(view);

			view.OnCallButtonPressed -= ViewOnCallButtonPressed;
			view.OnNavigationButtonPressed -= ViewOnNavigationButtonPressed;
			view.OnLocalButtonPressed -= ViewOnLocalButtonPressed;
			view.OnHangupButtonPressed -= ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed -= ViewOnRecentButtonPressed;
			view.OnBackButtonPressed -= ViewOnBackButtonPressed;
			view.OnHomeButtonPressed -= ViewOnHomeButtonPressed;
			view.OnDirectoryButtonPressed -= ViewOnDirectoryButtonPressed;
			view.OnManualDialButtonPressed -= ViewOnManualDialButtonPressed;

			view.OnDPadDownButtonPressed -= ViewOnDPadDownButtonPressed;
			view.OnDPadLeftButtonPressed -= ViewOnDPadLeftButtonPressed;
			view.OnDPadRightButtonPressed -= ViewOnDPadRightButtonPressed;
			view.OnDPadUpButtonPressed -= ViewOnDPadUpButtonPressed;
			view.OnDPadSelectButtonPressed -= ViewOnDPadSelectButtonPressed;
		}

		private void ViewOnDPadSelectButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Select);
		}

		private void ViewOnDPadUpButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Up);
		}

		private void ViewOnDPadRightButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Right);
		}

		private void ViewOnDPadLeftButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Left);
		}

		private void ViewOnDPadDownButtonPressed(object sender, EventArgs eventArgs)
		{
			if (m_ButtonComponent != null)
				m_ButtonComponent.PressButton(ButtonComponent.eDPad.Down);
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
		private void ViewOnDirectoryButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eMisc.Directory);
					break;
			}
		}

		/// <summary>
		/// Called when the user presses the home button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnHomeButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eMisc.Home);
					break;

				case ePolycomDirectoryMode.Local:
					m_DirectoryBrowser.GoToRoot();
					break;
			}
		}

		/// <summary>
		/// Called when the user presses the back button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnBackButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eCall.Back);
					break;

				case ePolycomDirectoryMode.Local:
					m_DirectoryBrowser.GoUp();
					break;
			}
		}

		/// <summary>
		/// Called when the user presses the recents button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnRecentButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = ePolycomDirectoryMode.Recents;
		}

		/// <summary>
		/// Called when the user presses the local button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnLocalButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = ePolycomDirectoryMode.Local;
		}

		/// <summary>
		/// Called when the user presses the dpad button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnNavigationButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = ePolycomDirectoryMode.Navigation;
		}

		/// <summary>
		/// Called when the user presses the call button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCallButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eCall.Call);
					break;

				case ePolycomDirectoryMode.Local:
				case ePolycomDirectoryMode.Recents:
					if (m_Selected == null)
						return;

					IVtcReferencedContactsPresenterBase presenter = m_Selected;
					Selected = null;

					presenter.Dial();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Called when the user presses the hangup button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			switch (DirectoryMode)
			{
				case ePolycomDirectoryMode.Navigation:
					if (m_ButtonComponent != null)
						m_ButtonComponent.PressButton(ButtonComponent.eCall.Hangup);
					break;

				case ePolycomDirectoryMode.Local:
				case ePolycomDirectoryMode.Recents:
					if (m_SubscribedConferenceManager == null || m_SubscribedConferenceManager.IsInCall == eInCall.None)
						return;

					// Go to the list of active calls
					Navigation.NavigateTo<IVtcButtonListPresenter>().ShowMenu(VtcButtonListPresenter.INDEX_ACTIVE_CALLS);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
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
			m_DirectoryBrowser.GoToRoot();
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
