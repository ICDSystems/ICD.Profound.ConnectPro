using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Conferencing.DialContexts;
using ICD.Connect.Conferencing.Directory;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.Conferencing.Favorites;
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
		private readonly List<IContact> m_Contacts;
		private readonly IcdOrderedDictionary<string, IContact> m_SelectedContacts;
		private readonly SafeTimer m_DebounceTimer;

		private string m_Filter;

		/// <summary>
		/// used to store filter when user hits enter so cancelling can return to old filter.
		/// </summary>
		private string m_ConfirmedFilter;

		private bool m_ShowFavorites;

		/// <summary>
		/// Gets/sets the active contact filter.
		/// </summary>
		private string Filter
		{
			get { return m_Filter; }
			set
			{
				if (value == m_Filter)
					return;

				m_Filter = value;

				RebuildContacts();
			}
		}

		/// <summary>
		/// When true show the favorites, otherwise show directory.
		/// </summary>
		private bool ShowFavorites
		{
			get { return m_ShowFavorites; }
			set
			{
				if (value == m_ShowFavorites)
					return;

				m_ShowFavorites = value;

				RebuildContacts();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WtcContactListPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ContactSection = new SafeCriticalSection();

			m_ContactFactory = new WtcReferencedContactPresenterFactory(nav, ContactItemFactory, Subscribe, Unsubscribe);
			m_SelectedContactFactory = new WtcReferencedSelectedContactPresenterFactory(nav, SelectedContactItemFactory, Subscribe, Unsubscribe);
			
			m_Contacts = new List<IContact>();
			m_SelectedContacts = new IcdOrderedDictionary<string, IContact>();

			m_DirectoryBrowser = new DirectoryControlBrowser();
			m_DirectoryBrowser.OnPathContentsChanged += DirectoryBrowserOnPathContentsChanged;

			m_DebounceTimer = SafeTimer.Stopped(RefreshIfVisible);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_DirectoryBrowser.Dispose();
			m_ContactFactory.Dispose();
			m_SelectedContactFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcContactListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string label =
					!string.IsNullOrEmpty(Filter)
						? string.Format("Contact List - Filter: \"{0}\"", Filter)
						: ShowFavorites
							? "Favorite List"
							: "Contact list";

				view.SetContactListLabelText(label);
				view.SetSearchButtonEnabled(true);
				view.SetFavoritesButtonSelected(string.IsNullOrEmpty(Filter) && ShowFavorites);

				IContact[] contacts = ExceptSelectedContacts(m_Contacts).ToArray();

				view.ShowNoContactsSelectedLabel(m_SelectedContacts.Count == 0);
				view.SetInviteParticipantButtonEnabled(m_SelectedContacts.Count > 0);

				foreach (IWtcReferencedContactPresenter presenter in m_ContactFactory.BuildChildren(contacts))
					presenter.ShowView(true);

				foreach (IWtcReferencedSelectedContactPresenter presenter in m_SelectedContactFactory.BuildChildren(m_SelectedContacts.Values))
					presenter.ShowView(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Filters selected contacts out of the given sequence.
		/// </summary>
		/// <param name="contacts"></param>
		/// <returns></returns>
		private IEnumerable<IContact> ExceptSelectedContacts([NotNull] IEnumerable<IContact> contacts)
		{
			if (contacts == null)
				throw new ArgumentNullException("contacts");

			return contacts.Where(c => !m_SelectedContacts.ContainsKey(c.Name));
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

		private IEnumerable<IContact> GetSelectedContacts()
		{
			return m_ContactSection.Execute(() => m_SelectedContacts.Values.ToArray());
		}

		private void AddSelectedContact(IContact contact)
		{
			if (contact == null)
				throw new ArgumentNullException("contact");

			m_ContactSection.Execute(() => m_SelectedContacts[contact.Name] = contact);
			RefreshIfVisible();
		}

		private void RemoveSelectedContact(IContact contact)
		{
			if (contact == null)
				throw new ArgumentNullException("contact");

			if (m_ContactSection.Execute(() => m_SelectedContacts.Remove(contact.Name)))
				RefreshIfVisible();
		}

		private void ClearSelectedContacts()
		{
			m_ContactSection.Enter();

			try
			{
				if (m_SelectedContacts.Count == 0)
					return;

				m_SelectedContacts.Clear();
			}
			finally
			{
				m_ContactSection.Leave();
			}

			RefreshIfVisible();
		}

		private void ConfirmFilter(string filter)
		{
			m_ConfirmedFilter = filter;
			Filter = filter;
		}

		private void PreviewFilter(string filter)
		{
			m_DebounceTimer.Reset(KEYBOARD_DEBOUNCE_TIME);
			Filter = filter;
		}

		private void CancelFilter()
		{
			Filter = m_ConfirmedFilter;
		}

		#endregion

		#region Directory Browser Callbacks

		private void DirectoryBrowserOnPathContentsChanged(object sender, EventArgs e)
		{
			RebuildContacts();
		}

		private void RebuildContacts()
		{
			m_ContactSection.Enter();

			try
			{
				IEnumerable<IContact> contacts = GetContacts();

				m_Contacts.Clear();
				m_Contacts.AddRange(contacts);
			}
			finally
			{
				m_ContactSection.Leave();
			}

			RefreshIfVisible();
		}

		private IEnumerable<IContact> GetContacts()
		{
			if (Room == null)
				return Enumerable.Empty<IContact>();

			if (m_DirectoryBrowser == null)
				return Enumerable.Empty<IContact>();

			IDirectoryFolder current = m_DirectoryBrowser.GetCurrentFolder();
			if (current == null)
				return Enumerable.Empty<IContact>();

			// Search
			if (!string.IsNullOrEmpty(Filter))
			{
				var nameScoringMethod = GetWeightedTokenSearchFunc(Filter);
				return current.GetContacts()
				              .GroupBy(c => nameScoringMethod(c.Name)) // group so we can filter low scores
				              .Where(g => g.Key > 0) // filter out non-matches
				              .OrderByDescending(g => g.Key)
				              .SelectMany(g => g) // ungroup
				              .OrderBy(c => c.Name);
			}

			// Favorites
			if (ShowFavorites)
			{
				IConferenceManager manager = Room.ConferenceManager;
				IFavorites favorites = manager == null ? null : manager.Favorites;
				if (favorites == null)
					return Enumerable.Empty<IContact>();

				IEnumerable<Favorite> zoomContacts = favorites.GetFavoritesByProtocol(eDialProtocol.ZoomContact);
				IEnumerable<Favorite> zoom = favorites.GetFavoritesByProtocol(eDialProtocol.Zoom);

				return zoomContacts.Concat(zoom)
				                   .OrderBy(c => c.Name)
				                   .Cast<IContact>();
			}

			// Directory
			return current.GetContacts()
			              .OrderBy(c =>
			              {
				              var onlineContact = c as IContactWithOnlineState;
				              if (onlineContact == null || onlineContact.OnlineState == eOnlineState.Unknown)
					              return eOnlineState.Offline;
				              return onlineContact.OnlineState;
			              })
			              .ThenBy(c => c.Name);
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

		#endregion

		#region Contact Callbacks

		private void Subscribe(IWtcReferencedContactPresenter presenter)
		{
			presenter.OnPressed += ContactOnPressed;
			presenter.OnIsFavoritedStateChanged += ContactOnIsFavoritedStateChanged;
		}

		private void Unsubscribe(IWtcReferencedContactPresenter presenter)
		{
			presenter.OnPressed -= ContactOnPressed;
			presenter.OnIsFavoritedStateChanged -= ContactOnIsFavoritedStateChanged;
		}

		private void ContactOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedContactPresenter;
			if (presenter == null)
				return;

			AddSelectedContact(presenter.Contact);
		}

		private void ContactOnIsFavoritedStateChanged(object sender, BoolEventArgs e)
		{
			if (ShowFavorites)
				RebuildContacts();
		}

		#endregion

		#region Selected Contact Callbacks

		private void Subscribe(IWtcReferencedSelectedContactPresenter presenter)
		{
			presenter.OnRemoveContact += SelectedContactOnRemovePressed;
		}

		private void Unsubscribe(IWtcReferencedSelectedContactPresenter presenter)
		{
			presenter.OnRemoveContact -= SelectedContactOnRemovePressed;
		}

		private void SelectedContactOnRemovePressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedSelectedContactPresenter;
			if (presenter == null)
				return;

			RemoveSelectedContact(presenter.Contact);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected override void Subscribe(IWebConferenceDeviceControl control)
		{
			base.Subscribe(control);

			if (control == null)
				return;

			IDirectoryControl directoryControl = control.Parent.Controls.GetControl<IDirectoryControl>();
			if (directoryControl == null)
				return;

			m_DirectoryBrowser.SetControl(directoryControl);
			m_DirectoryBrowser.PopulateCurrentFolder();

			RebuildContacts();
		}

		/// <summary>
		/// Unsusbcribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
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

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWtcContactListView view)
		{
			base.Subscribe(view);

			view.OnInviteParticipantButtonPressed += ViewOnInviteParticipantButtonPressed;
			view.OnSearchButtonPressed += ViewOnSearchButtonPressed;
			view.OnFavoritesButtonPressed += ViewOnFavoritesButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWtcContactListView view)
		{
			base.Unsubscribe(view);

			view.OnInviteParticipantButtonPressed -= ViewOnInviteParticipantButtonPressed;
			view.OnSearchButtonPressed -= ViewOnSearchButtonPressed;
			view.OnFavoritesButtonPressed -= ViewOnFavoritesButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the invite participants button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnInviteParticipantButtonPressed(object sender, EventArgs e)
		{
			if (ActiveConferenceControl == null)
				return;

			IContact[] contacts = GetSelectedContacts().ToArray();
			if (contacts.Length == 0)
				return;

			foreach (IContact contact in contacts)
			{
				IDialContext bestDialContext =
					contact.GetDialContexts()
					       .OrderByDescending(d => ActiveConferenceControl.CanDial(d))
					       .FirstOrDefault(d => ActiveConferenceControl.CanDial(d) != eDialContextSupport.Unsupported);

				if (bestDialContext != null)
					ActiveConferenceControl.Dial(bestDialContext);
			}

			ClearSelectedContacts();

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show("Invitation sent.", 1000);

			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the user presses the search button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ViewOnSearchButtonPressed(object sender, EventArgs e)
		{
			Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>()
			          .ShowView("Filter Contacts", m_ConfirmedFilter, KeyboardOnEnterPressed, KeyboardOnClosePressed,
			                    KeyboardOnStringChanged);
		}

		/// <summary>
		/// Called when the user presses the favorites button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnFavoritesButtonPressed(object sender, EventArgs eventArgs)
		{
			// Clear the filter - Skip the property to avoid a second refresh
			m_Filter = null;
			m_ConfirmedFilter = null;

			ShowFavorites = !ShowFavorites;
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			Filter = null;
			ShowFavorites = false;
			m_ConfirmedFilter = null;
			m_SelectedContacts.Clear();

			Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>().ShowView(false);
		}

		#endregion
	}
}
