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
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public sealed class WtcContactListPresenter : AbstractWtcPresenter<IWtcContactListView>, IWtcContactListPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly WtcReferencedDirectoryItemPresenterFactory m_PresenterFactory;
		private readonly DirectoryControlBrowser m_DirectoryBrowser;
		
		private IWtcReferencedDirectoryItemPresenter m_SelectedDirectoryItem;

		public WtcContactListPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new WtcReferencedDirectoryItemPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
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
				view.SetInviteParticipantButtonEnabled(m_SelectedDirectoryItem != null);

				var contacts = GetContacts();
				foreach (var presenter in m_PresenterFactory.BuildChildren(contacts))
				{
					presenter.Selected = presenter == m_SelectedDirectoryItem;
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

		private IEnumerable<IWtcReferencedDirectoryItemView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		private IEnumerable<DirectoryItem> GetContacts()
		{
			
			if (m_DirectoryBrowser == null)
				return Enumerable.Empty<DirectoryItem>();

			IDirectoryFolder current = m_DirectoryBrowser.GetCurrentFolder();
			if (current == null)
				return Enumerable.Empty<DirectoryItem>();

			return current
				.GetFolders()
				.Select(f => new DirectoryItem(f))
				.Concat(current.GetContacts().Select(c => new DirectoryItem(c)))
				.OrderBy(c => c.ModelType == DirectoryItem.eModelType.Contact)
				.ThenBy(c =>
				{
					if (c.ModelType == DirectoryItem.eModelType.Contact)
						return (c.Model as IContact).Name;
					if (c.ModelType == DirectoryItem.eModelType.Folder)
						return (c.Model as IDirectoryFolder).Name;

					// This should never happen
					throw new InvalidOperationException();
				});
		}

		#endregion

		#region Contact Callbacks

		private void Subscribe(IWtcReferencedDirectoryItemPresenter presenter)
		{
			presenter.OnPressed += PresenterOnOnPressed;
		}

		private void Unsubscribe(IWtcReferencedDirectoryItemPresenter presenter)
		{
			presenter.OnPressed -= PresenterOnOnPressed;
		}

		private void PresenterOnOnPressed(object sender, EventArgs eventArgs)
		{
			var presenter = sender as IWtcReferencedDirectoryItemPresenter;
			if (presenter == null)
				return;

			if (m_SelectedDirectoryItem == presenter)
				m_SelectedDirectoryItem = null;

			else if (presenter.DirectoryItem.ModelType == DirectoryItem.eModelType.Folder)
				m_DirectoryBrowser.EnterFolder(presenter.DirectoryItem.Model as IDirectoryFolder);

			else if (presenter.DirectoryItem.ModelType == DirectoryItem.eModelType.Contact)
				m_SelectedDirectoryItem = presenter;

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
			if (ActiveConferenceControl == null || m_SelectedDirectoryItem == null || m_SelectedDirectoryItem.DirectoryItem.ModelType != DirectoryItem.eModelType.Contact)
				return;

			var contact = m_SelectedDirectoryItem.DirectoryItem.Model as IContact;
			if (contact == null || !contact.GetDialContexts().Any())
				return;

			var bestDialContext = contact.GetDialContexts()
				.OrderByDescending(d => ActiveConferenceControl.CanDial(d)).FirstOrDefault();
			if (bestDialContext == null ||
			    ActiveConferenceControl.CanDial(bestDialContext) == eDialContextSupport.Unsupported)
				return;
			
			ActiveConferenceControl.Dial(bestDialContext);
		}
		private void ViewOnOnBackButtonPressed(object sender, EventArgs e)
		{
			m_DirectoryBrowser.GoUp();
			m_SelectedDirectoryItem = null;
			RefreshIfVisible();
		}

		#endregion
	}
}