using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference.Contacts
{
	public sealed class VtcContactsPresenter : AbstractPresenter<IVtcContactsView>, IVtcContactsPresenter
	{
		private enum eDirectoryMode
		{
			Contacts,
			Favorites,
			Recents
		}

		private readonly SafeCriticalSection m_RefreshSection;

		private eDirectoryMode m_DirectoryMode;

		/// <summary>
		/// Gets/sets the directory mode for populating the contacts list.
		/// </summary>
		private eDirectoryMode DirectoryMode
		{
			get { return m_DirectoryMode; }
			set
			{
				if (value == m_DirectoryMode)
					return;

				m_DirectoryMode = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcContactsPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Refresh the visual state of the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcContactsView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetDirectoryButtonSelected(m_DirectoryMode == eDirectoryMode.Contacts);
				view.SetFavoritesButtonSelected(m_DirectoryMode == eDirectoryMode.Favorites);
				view.SetRecentButtonSelected(m_DirectoryMode == eDirectoryMode.Recents);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcContactsView view)
		{
			base.Subscribe(view);

			view.OnTextEntered += ViewOnTextEntered;
			view.OnCallButtonPressed += ViewOnCallButtonPressed;
			view.OnDirectoryButtonPressed += ViewOnDirectoryButtonPressed;
			view.OnFavoritesButtonPressed += ViewOnFavoritesButtonPressed;
			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed += ViewOnRecentButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcContactsView view)
		{
			base.Unsubscribe(view);

			view.OnTextEntered += ViewOnTextEntered;
			view.OnCallButtonPressed += ViewOnCallButtonPressed;
			view.OnDirectoryButtonPressed += ViewOnDirectoryButtonPressed;
			view.OnFavoritesButtonPressed += ViewOnFavoritesButtonPressed;
			view.OnHangupButtonPressed += ViewOnHangupButtonPressed;
			view.OnRecentButtonPressed += ViewOnRecentButtonPressed;
		}

		private void ViewOnRecentButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Recents;
		}

		private void ViewOnFavoritesButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Favorites;
		}

		private void ViewOnDirectoryButtonPressed(object sender, EventArgs eventArgs)
		{
			DirectoryMode = eDirectoryMode.Contacts;
		}

		private void ViewOnCallButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnHangupButtonPressed(object sender, EventArgs eventArgs)
		{
			throw new NotImplementedException();
		}

		private void ViewOnTextEntered(object sender, StringEventArgs stringEventArgs)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
