using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Zoom.Components.Directory;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Conference.Contacts
{
	[PresenterBinding(typeof(IReferencedSelectedContactPresenter))]
	public sealed class ReferencedSelectedContactPresenter : AbstractTouchDisplayComponentPresenter<IReferencedSelectedContactView>, IReferencedSelectedContactPresenter
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

		public ReferencedSelectedContactPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		public override void Dispose()
		{
			base.Dispose();

			OnRemoveContact = null;
		}

		protected override void Refresh(IReferencedSelectedContactView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetContactName(Contact == null ? "Missing Contact Name" : Contact.Name);
				
				var zoomContact = Contact as ZoomContact;
				view.SetAvatarImageVisibility(true);
				view.SetAvatarImagePath(zoomContact == null || string.IsNullOrEmpty(zoomContact.AvatarUrl)
					? "ic_zoom_participants_head"
					: zoomContact.AvatarUrl);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		protected override void Subscribe(IReferencedSelectedContactView view)
		{
			base.Subscribe(view);

			view.OnRemovePressed += ViewOnOnRemovePressed;
		}

		protected override void Unsubscribe(IReferencedSelectedContactView view)
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