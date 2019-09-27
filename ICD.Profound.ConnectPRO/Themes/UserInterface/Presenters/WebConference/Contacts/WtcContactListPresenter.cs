﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
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
		private readonly List<IContact> m_Contacts;
		private readonly List<IContact> m_SelectedContacts;
		private readonly SafeTimer m_DebounceTimer;

		private string m_Filter;

		/// <summary>
		/// used to store filter when user hits enter so cancelling can return to old filter.
		/// </summary>
		private string m_ConfirmedFilter;

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
			m_SelectedContacts = new List<IContact>();

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

		protected override void Refresh(IWtcContactListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string label =
					string.IsNullOrEmpty(Filter)
						? "Contact List"
						: string.Format("Contact List - Filter: \"{0}\"", Filter);

				view.SetContactListLabelText(label);
				view.SetSearchButtonEnabled(true);

				IContact[] contacts = m_Contacts.Except(m_SelectedContacts).ToArray();

				view.ShowNoContactsSelectedLabel(m_SelectedContacts.Count == 0);
				view.SetInviteParticipantButtonEnabled(m_SelectedContacts.Count > 0);

				foreach (IWtcReferencedContactPresenter presenter in m_ContactFactory.BuildChildren(contacts))
					presenter.ShowView(true);

				foreach (IWtcReferencedSelectedContactPresenter presenter in m_SelectedContactFactory.BuildChildren(m_SelectedContacts))
					presenter.ShowView(true);
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

		private IEnumerable<IContact> GetSelectedContacts()
		{
			return m_ContactSection.Execute(() => m_SelectedContacts.ToArray());
		}

		private void AddSelectedContact(IContact contact)
		{
			if (m_ContactSection.Execute(() => m_SelectedContacts.AddSorted(contact, c => c.Name)))
				RefreshIfVisible();
		}

		private void RemoveSelectedContact(IContact contact)
		{
			if (m_ContactSection.Execute(() => m_SelectedContacts.RemoveSorted(contact, c => c.Name)))
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
			if (m_DirectoryBrowser == null)
				return Enumerable.Empty<IContact>();

			IDirectoryFolder current = m_DirectoryBrowser.GetCurrentFolder();
			if (current == null)
				return Enumerable.Empty<IContact>();

			if (string.IsNullOrEmpty(Filter))
				return current.GetContacts()
				              .OrderBy(c =>
				              {
					              var onlineContact = c as IContactWithOnlineState;
					              if (onlineContact == null || onlineContact.OnlineState == eOnlineState.Unknown)
						              return eOnlineState.Offline;
					              return onlineContact.OnlineState;
				              })
				              .ThenBy(c => c.Name);

			var nameScoringMethod = GetWeightedTokenSearchFunc(Filter);
			return current.GetContacts()
			              .GroupBy(c => nameScoringMethod(c.Name)) // group so we can filter low scores
			              .Where(g => g.Key > 0) // filter out non-matches
			              .OrderByDescending(g => g.Key)
						  .SelectMany(g => g) // ungroup
						  .OrderBy(c => c.Name);
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
		}

		private void Unsubscribe(IWtcReferencedContactPresenter presenter)
		{
			presenter.OnPressed -= ContactOnPressed;
		}

		private void ContactOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedContactPresenter;
			if (presenter == null)
				return;

			AddSelectedContact(presenter.Contact);
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

			RebuildContacts();
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

			view.OnInviteParticipantButtonPressed += ViewOnInviteParticipantButtonPressed;
			view.OnSearchButtonPressed += ViewOnSearchButtonPressed;
		}

		protected override void Unsubscribe(IWtcContactListView view)
		{
			base.Unsubscribe(view);

			view.OnInviteParticipantButtonPressed -= ViewOnInviteParticipantButtonPressed;
			view.OnSearchButtonPressed -= ViewOnSearchButtonPressed;
		}

		private void ViewOnInviteParticipantButtonPressed(object sender, EventArgs e)
		{
			if (ActiveConferenceControl == null)
				return;

			IContact[] contacts = GetSelectedContacts().ToArray();
			if (contacts.Length == 0)
				return;

			foreach (var contact in contacts)
			{
				if (contact == null || !contact.GetDialContexts().Any())
					continue;

				var bestDialContext = contact.GetDialContexts()
				                             .OrderByDescending(d => ActiveConferenceControl.CanDial(d))
				                             .FirstOrDefault();
				if (bestDialContext == null ||
				    ActiveConferenceControl.CanDial(bestDialContext) == eDialContextSupport.Unsupported)
					continue;

				ActiveConferenceControl.Dial(bestDialContext);
				RemoveSelectedContact(contact);
			}

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>()
			          .Show("Invitation sent.", 1000);

			RefreshIfVisible();
		}

		private void ViewOnSearchButtonPressed(object sender, EventArgs e)
		{
			Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>()
			          .ShowView("Filter Contacts", m_ConfirmedFilter, KeyboardOnEnterPressed, KeyboardOnClosePressed,
			                    KeyboardOnStringChanged);
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			Filter = null;
			m_ConfirmedFilter = null;
			m_SelectedContacts.Clear();

			Navigation.LazyLoadPresenter<IGenericKeyboardPresenter>().ShowView(false);
		}

		#endregion
	}
}