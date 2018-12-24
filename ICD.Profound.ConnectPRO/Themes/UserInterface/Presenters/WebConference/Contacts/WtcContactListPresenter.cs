using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Conferencing.Directory;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.Devices;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public sealed class WtcContactListPresenter : AbstractWtcPresenter<IWtcContactListView>, IWtcContactListPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly WtcReferencedContactPresenterFactory m_PresenterFactory;
		private readonly DirectoryControlBrowser m_DirectoryBrowser;
		
		private IWtcReferencedContactPresenter m_SelectedContact;

		public WtcContactListPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new WtcReferencedContactPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
			m_DirectoryBrowser = new DirectoryControlBrowser();

			m_DirectoryBrowser.OnPathContentsChanged += DirectoryBrowserOnOnPathContentsChanged;
		}

		private void DirectoryBrowserOnOnPathContentsChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		public override void Dispose()
		{
			m_DirectoryBrowser.Dispose();
			m_PresenterFactory.Dispose();

			base.Dispose();
		}

		protected override void Refresh(IWtcContactListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetBackButtonEnabled(!m_DirectoryBrowser.IsCurrentFolderRoot);

				var contact = m_SelectedContact == null ? null : m_SelectedContact.Contact;
				var onlineContact = contact == null ? null : contact as IContactWithOnlineState;
				var inviteEnabled = onlineContact == null
					? contact != null
					: onlineContact.OnlineState != eOnlineState.Offline;
				view.SetInviteParticipantButtonEnabled(inviteEnabled);

				var contacts = GetContacts();
				foreach (var presenter in m_PresenterFactory.BuildChildren(contacts))
				{
					presenter.Selected = presenter == m_SelectedContact;
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

		private IEnumerable<IWtcReferencedContactView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		private IEnumerable<IContact> GetContacts()
		{
			
			if (m_DirectoryBrowser == null)
				return Enumerable.Empty<IContact>();

			IDirectoryFolder current = m_DirectoryBrowser.GetCurrentFolder();
			if (current == null)
				return Enumerable.Empty<IContact>();

			return current.GetContacts()
				.OrderByDescending(c =>
				{
					var onlineContact = c as IContactWithOnlineState;
					if (onlineContact == null)
						return eOnlineState.Offline;
					return onlineContact.OnlineState;
				})
				.ThenBy(c => c.Name);
		}

		#endregion

		#region Contact Callbacks

		private void Subscribe(IWtcReferencedContactPresenter presenter)
		{
			presenter.OnPressed += PresenterOnOnPressed;
		}

		private void Unsubscribe(IWtcReferencedContactPresenter presenter)
		{
			presenter.OnPressed -= PresenterOnOnPressed;
		}

		private void PresenterOnOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedContactPresenter;
			if (presenter == null)
				return;

			m_SelectedContact = m_SelectedContact == presenter ? null : presenter;

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

		#region View Callbacks

		protected override void Subscribe(IWtcContactListView view)
		{
			base.Subscribe(view);

			view.OnInviteParticipantButtonPressed += ViewOnOnInviteParticipantButtonPressed;
			view.OnBackButtonPressed += ViewOnOnBackButtonPressed;
		}

		protected override void Unsubscribe(IWtcContactListView view)
		{
			base.Unsubscribe(view);

			view.OnInviteParticipantButtonPressed -= ViewOnOnInviteParticipantButtonPressed;
			view.OnBackButtonPressed -= ViewOnOnBackButtonPressed;
		}

		private void ViewOnOnInviteParticipantButtonPressed(object sender, EventArgs e)
		{
			if (ActiveConferenceControl == null || m_SelectedContact == null || m_SelectedContact.Contact == null)
				return;

			var contact = m_SelectedContact.Contact;
			if (contact == null || !contact.GetDialContexts().Any())
				return;

			var bestDialContext = contact.GetDialContexts()
				.OrderByDescending(d => ActiveConferenceControl.CanDial(d)).FirstOrDefault();
			if (bestDialContext == null ||
			    ActiveConferenceControl.CanDial(bestDialContext) == eDialContextSupport.Unsupported)
				return;
			
			ActiveConferenceControl.Dial(bestDialContext);

			Navigation.LazyLoadPresenter<IGenericAlertPresenter>().Show("Invitation sent.", 1000);
		}
		private void ViewOnOnBackButtonPressed(object sender, EventArgs e)
		{
			m_DirectoryBrowser.GoUp();
			m_SelectedContact = null;
			RefreshIfVisible();
		}

		#endregion
	}
}