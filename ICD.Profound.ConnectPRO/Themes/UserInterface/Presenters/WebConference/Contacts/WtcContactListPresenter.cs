using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Conferencing.Directory;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.Devices;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	[PresenterBinding(typeof(IWtcContactListPresenter))]
	public sealed class WtcContactListPresenter : AbstractWtcPresenter<IWtcContactListView>, IWtcContactListPresenter
	{
		private const long KEYBOARD_DEBOUNCE_TIME = 1000;

		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeCriticalSection m_ContactSection;
		private readonly WtcReferencedContactPresenterFactory m_ContactFactory;
		private readonly WtcReferencedSelectedContactPresenterFactory m_SelectedContactFactory;
		private readonly DirectoryControlBrowser m_DirectoryBrowser;
		private readonly List<IContact> m_SelectedContacts;
		private readonly SafeTimer m_DebounceTimer;

		private string m_Filter = "";
		private string m_ConfirmedFilter = ""; //used to store filter when user hits enter so cancelling can return to old filter

		public WtcContactListPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ContactSection = new SafeCriticalSection();
			m_ContactFactory = new WtcReferencedContactPresenterFactory(nav, ContactItemFactory, Subscribe, Unsubscribe);
			m_SelectedContactFactory = new WtcReferencedSelectedContactPresenterFactory(nav, SelectedContactItemFactory, Subscribe, Unsubscribe);
			m_SelectedContacts = new List<IContact>();

			m_DirectoryBrowser = new DirectoryControlBrowser();
			m_DirectoryBrowser.OnPathContentsChanged += DirectoryBrowserOnOnPathContentsChanged;

			m_DebounceTimer = SafeTimer.Stopped(RefreshIfVisible);
		}

		private void DirectoryBrowserOnOnPathContentsChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		public override void Dispose()
		{
			m_DirectoryBrowser.Dispose();
			m_ContactFactory.Dispose();
			m_SelectedContactFactory.Dispose();

			base.Dispose();
		}

		protected override void Refresh(IWtcContactListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetContactListLabelText(string.IsNullOrEmpty(m_Filter) ? "Contact List" : string.Format("Contact List - Filter: \"{0}\"", m_Filter));
				var contacts = GetContacts().ToList();
				view.SetSearchButtonEnabled(true);

				var selectedContacts = GetSelectedContacts().ToList();
				view.SetInviteParticipantButtonEnabled(selectedContacts.Any());
				foreach (var presenter in m_ContactFactory.BuildChildren(contacts))
				{
					presenter.ShowView(true);
					presenter.Refresh();
				}

				foreach (var presenter in m_SelectedContactFactory.BuildChildren(selectedContacts))
				{
					presenter.ShowView(true);
					presenter.Refresh();
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		private IEnumerable<IWtcReferencedContactView> ContactItemFactory(ushort count)
		{
			return GetView().GetContactViews(ViewFactory, count);
		}

		private IEnumerable<IWtcReferencedSelectedContactView> SelectedContactItemFactory(ushort count)
		{
			return GetView().GetSelectedContactViews(ViewFactory, count);
		}

		private IEnumerable<IContact> GetContacts()
		{
			
			if (m_DirectoryBrowser == null)
				return Enumerable.Empty<IContact>();

			IDirectoryFolder current = m_DirectoryBrowser.GetCurrentFolder();
			if (current == null)
				return Enumerable.Empty<IContact>();

			if (string.IsNullOrEmpty(m_Filter))
				return current.GetContacts()
				              .OrderBy(c =>
				                       {
					                       var onlineContact = c as IContactWithOnlineState;
					                       if (onlineContact == null || onlineContact.OnlineState == eOnlineState.Unknown)
						                       return eOnlineState.Offline;
					                       return onlineContact.OnlineState;
				                       })
				              .ThenBy(c => c.Name);

			var nameScoringMethod = GetWeightedTokenSearchFunc(m_Filter);
			return current.GetContacts()
			              .GroupBy(c => nameScoringMethod(c.Name)) // group so we can filter low scores
			              .Where(g => g.Key > 0) // filter out non-matches
			              .OrderByDescending(g => g.Key)
			              .SelectMany(g => g); // ungroup
		}

		private static Func<string, double> GetWeightedTokenSearchFunc(string searchString)
		{
			if (searchString == null)
				throw new ArgumentNullException("searchString");

			string[] tokens = searchString.Split();
			return name => WeightedTokenSearch(name, searchString, tokens);
		}

		private static double WeightedTokenSearch(string name, string searchString, string[] searchTokens)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			double score = 0;
			// if full search string matches in name, tons of points
			if (name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
				score += 2.0;

			string[] nameTokens = name.Split();
			foreach (var searchToken in searchTokens)
			{
				if (string.IsNullOrEmpty(searchToken))
					continue;

				string token = searchToken;
				int[] matches = nameTokens.Select(n => n.IndexOf(token, StringComparison.OrdinalIgnoreCase)).ToArray();

				// if search token found in any name tokens, lots of points
				if (matches.Any(i => i != -1))
					score += 1.0;

				// if it matches the beginning of the name, decent points
				if (matches[0] == 0)
					score += 0.5;
				// else if it matches the beginning of any name token, small points
				else if (matches.Any(i => i == 0))
					score += 0.25;
			}
			return score;
		}

		private IEnumerable<IContact> GetSelectedContacts()
		{
			return m_ContactSection.Execute(() => m_SelectedContacts.ToList());
		}

		private void AddContact(IContact contact)
		{
			m_ContactSection.Enter();
			try
			{
				if (m_SelectedContacts.Contains(contact))
					return;

				m_SelectedContacts.Add(contact);
			}
			finally
			{
				m_ContactSection.Leave();
			}
		}

		private void RemoveContact(IContact contact)
		{
			m_ContactSection.Enter();
			try
			{
				if (!m_SelectedContacts.Contains(contact))
					return;
				m_SelectedContacts.Remove(contact);
			}
			finally
			{
				m_ContactSection.Leave();
			}
		}
		
		private void ConfirmFilter(string filter)
		{
			m_ConfirmedFilter = filter;
			m_Filter = filter;
			RefreshIfVisible();
		}

		private void PreviewFilter(string filter)
		{
			m_DebounceTimer.Reset(KEYBOARD_DEBOUNCE_TIME);
			m_Filter = filter;
		}

		private void CancelFilter()
		{
			m_Filter = m_ConfirmedFilter;
			RefreshIfVisible();
		}

		#endregion

		#region Contact Callbacks

		private void Subscribe(IWtcReferencedContactPresenter presenter)
		{
			presenter.OnPressed += PresenterOnPressed;
		}

		private void Unsubscribe(IWtcReferencedContactPresenter presenter)
		{
			presenter.OnPressed -= PresenterOnPressed;
		}

		private void PresenterOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedContactPresenter;
			if (presenter == null)
				return;

			AddContact(presenter.Contact);

			RefreshIfVisible();
		}

		#endregion

		#region Selected Contact Callbacks

		private void Subscribe(IWtcReferencedSelectedContactPresenter presenter)
		{
			presenter.OnRemoveContact += PresenterOnRemoveContact;
		}

		private void Unsubscribe(IWtcReferencedSelectedContactPresenter presenter)
		{
			presenter.OnRemoveContact -= PresenterOnRemoveContact;
		}

		private void PresenterOnRemoveContact(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedSelectedContactPresenter;
			if (presenter == null)
				return;

			RemoveContact(presenter.Contact);

			RefreshIfVisible();
		}

		#endregion

		#region Control Callbacks

		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			var device = control.Parent as IDevice;
			var directoryControl = device == null ? null : device.Controls.GetControl<IDirectoryControl>();
			if (directoryControl == null)
				return;

			m_DirectoryBrowser.SetControl(directoryControl);
			m_DirectoryBrowser.PopulateCurrentFolder();
		}

		protected override void Unsubscribe(IWebConferenceDeviceControl control)
		{
			base.Unsubscribe(control);

			m_DirectoryBrowser.SetControl(null);
		}

		#endregion

		#region Keyboard Callbacks

		private void KeyboardOnStringChanged(string text)
		{
			if (IsViewVisible)
				PreviewFilter(text);
		}

		private void KeyboardOnEnterPressed(string text)
		{
			if (IsViewVisible)
				ConfirmFilter(text);
		}

		private void KeyboardOnClosePressed(string text)
		{
			if (IsViewVisible)
				CancelFilter();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IWtcContactListView view)
		{
			base.Subscribe(view);

			view.OnInviteParticipantButtonPressed += ViewOnOnInviteParticipantButtonPressed;
			view.OnSearchButtonPressed += ViewOnOnSearchButtonPressed;
		}

		protected override void Unsubscribe(IWtcContactListView view)
		{
			base.Unsubscribe(view);

			view.OnInviteParticipantButtonPressed -= ViewOnOnInviteParticipantButtonPressed;
			view.OnSearchButtonPressed -= ViewOnOnSearchButtonPressed;
		}

		private void ViewOnOnInviteParticipantButtonPressed(object sender, EventArgs e)
		{
			if (ActiveConferenceControl == null)
				return;

			var contacts = GetSelectedContacts().ToList();
			if (!contacts.Any())
				return;

			foreach (var contact in contacts)
			{
				if (contact == null || !contact.GetDialContexts().Any())
					continue;

				var bestDialContext = contact.GetDialContexts()
					.OrderByDescending(d => ActiveConferenceControl.CanDial(d)).FirstOrDefault();
				if (bestDialContext == null ||
				    ActiveConferenceControl.CanDial(bestDialContext) == eDialContextSupport.Unsupported)
					continue;

				ActiveConferenceControl.Dial(bestDialContext);
				RemoveContact(contact);
			}

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>().Show("Invitation sent.", 1000);
			RefreshIfVisible();
		}

		private void ViewOnOnSearchButtonPressed(object sender, EventArgs e)
		{
			Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>()
			          .ShowView("Filter Contacts", m_ConfirmedFilter, KeyboardOnEnterPressed, KeyboardOnClosePressed,
			                    KeyboardOnStringChanged);
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_Filter = null;
			m_ConfirmedFilter = null;
			m_SelectedContacts.Clear();
		}

		#endregion
	}
}