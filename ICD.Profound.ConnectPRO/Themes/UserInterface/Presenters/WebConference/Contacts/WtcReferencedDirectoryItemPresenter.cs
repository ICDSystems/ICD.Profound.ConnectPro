using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Components.Directory;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	public class WtcReferencedDirectoryItemPresenter : AbstractUiComponentPresenter<IWtcReferencedDirectoryItemView>, IWtcReferencedDirectoryItemPresenter
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private DirectoryItem m_DirectoryItem;
		public DirectoryItem DirectoryItem
		{
			get { return m_DirectoryItem; }
			set
			{
				if (m_DirectoryItem == value)
					return;

				Unsubscribe(m_DirectoryItem);
				m_DirectoryItem = value;
				Subscribe(m_DirectoryItem);

				RefreshIfVisible();
			}
		}

		public bool Selected { get; set; }

		public WtcReferencedDirectoryItemPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		protected override void Refresh(IWtcReferencedDirectoryItemView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				if (DirectoryItem.ModelType == DirectoryItem.eModelType.Folder)
				{
					var folder = DirectoryItem.Model as IDirectoryFolder;
					view.SetContactName(folder == null ? "Missing Folder Name" : folder.Name);
					view.SetOnlineStateMode(eOnlineState.Offline);
					view.SetAvatarImageVisibility(false);
					view.SetAvatarImagePath(null);
				}
				else if (DirectoryItem.ModelType == DirectoryItem.eModelType.Contact)
				{
					var contact = DirectoryItem.Model as IContact;
					view.SetContactName(contact == null ? "Missing Contact Name" : contact.Name);

					var onlineContact = contact as IContactWithOnlineState;
					view.SetOnlineStateMode(onlineContact == null ? eOnlineState.Offline : onlineContact.OnlineState);

					var zoomContact = contact as ZoomContact;
					view.SetAvatarImageVisibility(zoomContact != null && !string.IsNullOrEmpty(zoomContact.AvatarUrl));
					view.SetAvatarImagePath(zoomContact == null ? null : zoomContact.AvatarUrl);
				}
				view.SetButtonSelected(Selected);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Contact Callbacks 

		private void Subscribe(DirectoryItem item)
		{
			var contact = item.Model as IContactWithOnlineState;
			if (contact != null)
				contact.OnOnlineStateChanged += ContactOnOnOnlineStateChanged;
		}

		private void Unsubscribe(DirectoryItem item)
		{
			var contact = item.Model as IContactWithOnlineState;
			if (contact != null)
				contact.OnOnlineStateChanged -= ContactOnOnOnlineStateChanged;
		}

		private void ContactOnOnOnlineStateChanged(object sender, OnlineStateEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IWtcReferencedDirectoryItemView view)
		{
			base.Subscribe(view);

			view.OnContactPressed += ViewOnOnContactPressed;
		}

		protected override void Unsubscribe(IWtcReferencedDirectoryItemView view)
		{
			base.Unsubscribe(view);

			view.OnContactPressed -= ViewOnOnContactPressed;
		}

		private void ViewOnOnContactPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}