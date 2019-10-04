using System;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Directory.Tree;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	[PresenterBinding(typeof(IVtcReferencedFolderPresenter))]
	public sealed class VtcReferencedFolderPresenter : AbstractVtcReferencedContactsPresenterBase, IVtcReferencedFolderPresenter
	{
		private IDirectoryFolder m_Folder;

		[CanBeNull]
		public IDirectoryFolder Folder
		{
			get { return m_Folder; }
			set
			{
				if (value == m_Folder)
					return;

				m_Folder = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Can't select a folder.
		/// </summary>
		public override bool Selected { get { return false; } set { } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcReferencedFolderPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Override to control the display name for this contact.
		/// </summary>
		/// <returns></returns>
		protected override string GetName()
		{
			return Folder == null ? null : Folder.Name;
		}

		/// <summary>
		/// Override to control the favorite state for this contact.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavorite()
		{
			return false;
		}

		/// <summary>
		/// Override to control the visibility of the favorite button for this contact.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsFavoriteVisible()
		{
			return false;
		}

		protected override void SetModel(object model)
		{
			Folder = model as IDirectoryFolder;
		}

		protected override void Dial()
		{
			// Can't dial a folder
			throw new NotSupportedException();
		}
	}
}
