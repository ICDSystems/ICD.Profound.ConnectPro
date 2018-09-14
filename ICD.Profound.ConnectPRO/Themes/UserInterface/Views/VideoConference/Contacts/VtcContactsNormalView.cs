using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts
{
	public sealed partial class VtcContactsNormalView : AbstractVtcContactsView, IVtcContactsNormalView
	{
		/// <summary>
		/// Raised when the user presses the directory button.
		/// </summary>
		public override event EventHandler OnDirectoryButtonPressed;

		/// <summary>
		/// Raised when the user presses the favourites button.
		/// </summary>
		public event EventHandler OnFavoritesButtonPressed;

		/// <summary>
		/// Raised when the user presses the recent button.
		/// </summary>
		public override event EventHandler OnRecentButtonPressed;

		/// <summary>
		/// Raised when the user presses the call button.
		/// </summary>
		public override event EventHandler OnCallButtonPressed;

		/// <summary>
		/// Raised when the user presses the hangup button.
		/// </summary>
		public override event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Raised when the user presses the back button.
		/// </summary>
		public override event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Raised when the user presses the home button.
		/// </summary>
		public override event EventHandler OnHomeButtonPressed;

		/// <summary>
		/// Raised when the user presses the search button.
		/// </summary>
		public event EventHandler OnSearchButtonPressed;

		/// <summary>
		/// Raised when the user presses the manual dial button.
		/// </summary>
		public override event EventHandler OnManualDialButtonPressed;

		private readonly List<IVtcReferencedContactsView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcContactsNormalView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
			m_ChildList = new List<IVtcReferencedContactsView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDirectoryButtonPressed = null;
			OnFavoritesButtonPressed = null;
			OnRecentButtonPressed = null;
			OnCallButtonPressed = null;
			OnHangupButtonPressed = null;
			OnBackButtonPressed = null;
			OnHomeButtonPressed = null;
			OnSearchButtonPressed = null;
			OnManualDialButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override IEnumerable<IVtcReferencedContactsView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ContactList, m_ChildList, count);
		}

		public void SetDirectoryButtonSelected(bool selected)
		{
			m_DirectoryButton.SetSelected(selected);
		}

		public void SetFavoritesButtonSelected(bool selected)
		{
			m_FavoritesButton.SetSelected(selected);
		}

		public override void SetRecentButtonSelected(bool selected)
		{
			m_RecentsButton.SetSelected(selected);
		}

		public override void SetCallButtonEnabled(bool enabled)
		{
			m_CallButton.Enable(enabled);
		}

		public override void SetHangupButtonEnabled(bool enabled)
		{
			m_HangupButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the visibility of the navigation buttons.
		/// </summary>
		/// <param name="visible"></param>
		public void SetNavigationButtonsVisible(bool visible)
		{
			m_BackButton.Show(visible);
			m_HomeButton.Show(visible);

			// TODO
			m_SearchButton.Show(false);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DirectoryButton.OnPressed += DirectoryButtonOnPressed;
			m_FavoritesButton.OnPressed += FavoritesButtonOnPressed;
			m_RecentsButton.OnPressed += RecentsButtonOnPressed;
			m_CallButton.OnPressed += CallButtonOnPressed;
			m_HangupButton.OnPressed += HangupButtonOnPressed;
			m_BackButton.OnPressed += BackButtonOnPressed;
			m_HomeButton.OnPressed += HomeButtonOnPressed;
			m_SearchButton.OnPressed += SearchButtonOnPressed;
			m_ManualDialButton.OnPressed += ManualDialButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DirectoryButton.OnPressed -= DirectoryButtonOnPressed;
			m_FavoritesButton.OnPressed -= FavoritesButtonOnPressed;
			m_RecentsButton.OnPressed -= RecentsButtonOnPressed;
			m_CallButton.OnPressed -= CallButtonOnPressed;
			m_HangupButton.OnPressed -= HangupButtonOnPressed;
			m_BackButton.OnPressed -= BackButtonOnPressed;
			m_HomeButton.OnPressed -= HomeButtonOnPressed;
			m_SearchButton.OnPressed -= SearchButtonOnPressed;
			m_ManualDialButton.OnPressed -= ManualDialButtonOnPressed;
		}

		private void HangupButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnHangupButtonPressed.Raise(this);
		}

		private void CallButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCallButtonPressed.Raise(this);
		}

		private void DirectoryButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDirectoryButtonPressed.Raise(this);
		}

		private void RecentsButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnRecentButtonPressed.Raise(this);
		}

		private void FavoritesButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnFavoritesButtonPressed.Raise(this);
		}

		private void ManualDialButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnManualDialButtonPressed.Raise(this);
		}

		private void SearchButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSearchButtonPressed.Raise(this);
		}

		private void HomeButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnHomeButtonPressed.Raise(this);
		}

		private void BackButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnBackButtonPressed.Raise(this);
		}

		#endregion
	}
}
