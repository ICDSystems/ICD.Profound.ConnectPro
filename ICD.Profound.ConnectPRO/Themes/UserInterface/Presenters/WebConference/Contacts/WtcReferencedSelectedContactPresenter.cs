using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms.Components.Directory;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.Contacts;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.Contacts
{
	[PresenterBinding(typeof(IWtcReferencedSelectedContactPresenter))]
	public sealed class WtcReferencedSelectedContactPresenter : AbstractUiComponentPresenter<IWtcReferencedSelectedContactView>, IWtcReferencedSelectedContactPresenter
	{
		public event EventHandler OnRemoveContact;

		private readonly SafeCriticalSection m_RefreshSection;

		private IContact m_Contact;

		[CanBeNull]
		public IContact Contact
		{
			get { return m_Contact; }
			set
			{
				if (m_Contact == value)
					return;
				
				m_Contact = value;
				RefreshIfVisible();
			}
		}

		public WtcReferencedSelectedContactPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		public override void Dispose()
		{
			base.Dispose();

			OnRemoveContact = null;
		}

		protected override void Refresh(IWtcReferencedSelectedContactView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetContactName(Contact == null ? "Missing Contact Name" : Contact.Name);
				
				var zoomContact = Contact as ZoomContact;
				view.SetAvatarImageVisibility(zoomContact != null && !string.IsNullOrEmpty(zoomContact.AvatarUrl));
				view.SetAvatarImagePath(zoomContact == null ? null : zoomContact.AvatarUrl);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		protected override void Subscribe(IWtcReferencedSelectedContactView view)
		{
			base.Subscribe(view);

			view.OnRemovePressed += ViewOnOnRemovePressed;
		}

		protected override void Unsubscribe(IWtcReferencedSelectedContactView view)
		{
			base.Unsubscribe(view);

			view.OnRemovePressed -= ViewOnOnRemovePressed;
		}

		private void ViewOnOnRemovePressed(object sender, EventArgs e)
		{
			OnRemoveContact.Raise(this);
		}

		#endregion
	}
}