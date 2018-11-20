using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.Contacts;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Directory.Tree;
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

		public DirectoryItem DirectoryItem { get; set; }

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
				if(DirectoryItem.ModelType == DirectoryItem.eModelType.Folder)
					view.SetContactName((DirectoryItem.Model as IDirectoryFolder).Name);
				else if (DirectoryItem.ModelType == DirectoryItem.eModelType.Contact)
					view.SetContactName((DirectoryItem.Model as IContact).Name);
				view.SetButtonSelected(Selected);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

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