using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Zoom.Components.Directory;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	[PresenterBinding(typeof(IWtcReferencedContactPresenter))]
	public sealed class WtcReferencedContactPresenter : AbstractUiComponentPresenter<IWtcReferencedContactView>, IWtcReferencedContactPresenter
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private IContact m_Contact;
		private bool m_Selected;

		#region Properties

		public IContact Contact
		{
			get { return m_Contact; }
			set
			{
				if (value == m_Contact)
					return;

				Unsubscribe(m_Contact);
				m_Contact = value;
				Subscribe(m_Contact);

				RefreshIfVisible();
			}
		}

		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

				RefreshIfVisible();
			}
		}

		#endregion

		public WtcReferencedContactPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		protected override void Refresh(IWtcReferencedContactView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetContactName(Contact == null ? "Missing Contact Name" : Contact.Name);

				var onlineContact = Contact as IContactWithOnlineState;
				view.SetOnlineStateMode(onlineContact == null
					                        ? eOnlineState.Offline
					                        : onlineContact.OnlineState);

				var zoomContact = Contact as ZoomContact;
				view.SetAvatarImageVisibility(zoomContact != null &&
				                              !string.IsNullOrEmpty(zoomContact.AvatarUrl));
				view.SetAvatarImagePath(zoomContact == null ? null : zoomContact.AvatarUrl);
				view.SetButtonSelected(Selected);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Contact Callbacks 

		private void Subscribe(IContact item)
		{
			var contact = item as IContactWithOnlineState;
			if (contact != null)
				contact.OnOnlineStateChanged += ContactOnOnOnlineStateChanged;
		}

		private void Unsubscribe(IContact item)
		{
			var contact = item as IContactWithOnlineState;
			if (contact != null)
				contact.OnOnlineStateChanged -= ContactOnOnOnlineStateChanged;
		}

		private void ContactOnOnOnlineStateChanged(object sender, OnlineStateEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IWtcReferencedContactView view)
		{
			base.Subscribe(view);

			view.OnContactPressed += ViewOnContactPressed;
		}

		protected override void Unsubscribe(IWtcReferencedContactView view)
		{
			base.Unsubscribe(view);

			view.OnContactPressed -= ViewOnContactPressed;
		}

		private void ViewOnContactPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}