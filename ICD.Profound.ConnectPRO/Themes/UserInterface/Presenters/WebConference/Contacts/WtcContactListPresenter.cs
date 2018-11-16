using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Conferencing.Directory;
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
		private readonly WtcReferencedContactPresenterFactory m_PresenterFactory;
		private readonly DirectoryControlBrowser m_DirectoryBrowser;
		
		private IWtcReferencedContactPresenter m_SelectedContact;

		public WtcContactListPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new WtcReferencedContactPresenterFactory(nav, ItemFactory);
			m_DirectoryBrowser = new DirectoryControlBrowser();

			m_DirectoryBrowser.OnPathContentsChanged += DirectoryBrowserOnOnPathContentsChanged;
		}

		private void DirectoryBrowserOnOnPathContentsChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		protected override void Refresh(IWtcContactListView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				var contacts = m_DirectoryBrowser.GetCurrentFolder() == null 
					? Enumerable.Empty<IContact>() 
					: m_DirectoryBrowser.GetCurrentFolder().GetContacts();
				foreach (var presenter in m_PresenterFactory.BuildChildren(contacts, SubscribeChild, UnsubscribeChild))
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

		#endregion

		#region Contact Callbacks

		private void SubscribeChild(IWtcReferencedContactPresenter presenter)
		{
			if (presenter != null)
				presenter.OnPressed += PresenterOnOnPressed;
		}

		private void UnsubscribeChild(IWtcReferencedContactPresenter presenter)
		{
			if (presenter != null)
				presenter.OnPressed -= PresenterOnOnPressed;
		}

		private void PresenterOnOnPressed(object sender, EventArgs eventArgs)
		{
			
			var presenter = sender as IWtcReferencedContactPresenter;
			if (m_SelectedContact == presenter)
				m_SelectedContact = null;
			else
				m_SelectedContact = presenter;
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
		}

		protected override void Unsubscribe(IWtcContactListView view)
		{
			base.Unsubscribe(view);

			view.OnInviteParticipantButtonPressed -= ViewOnOnInviteParticipantButtonPressed;
		}

		private void ViewOnOnInviteParticipantButtonPressed(object sender, EventArgs e)
		{
			if (ActiveConferenceControl == null || m_SelectedContact == null || m_SelectedContact.Contact == null ||
			    !m_SelectedContact.Contact.GetDialContexts().Any())
				return;

			var bestDialContext = m_SelectedContact.Contact.GetDialContexts()
				.OrderByDescending(d => ActiveConferenceControl.CanDial(d)).FirstOrDefault();
			if (bestDialContext == null ||
			    ActiveConferenceControl.CanDial(bestDialContext) == eDialContextSupport.Unsupported)
				return;
			
			ActiveConferenceControl.Dial(bestDialContext);
		}

		#endregion
	}
}