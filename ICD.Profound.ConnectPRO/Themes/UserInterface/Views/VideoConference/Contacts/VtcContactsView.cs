using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts
{
	public sealed partial class VtcContactsView : AbstractView, IVtcContactsView
	{
		public event EventHandler<StringEventArgs> OnTextEntered;
		public event EventHandler OnDirectoryButtonPressed;
		public event EventHandler OnFavoritesButtonPressed;
		public event EventHandler OnRecentButtonPressed;
		public event EventHandler OnCallButtonPressed;
		public event EventHandler OnHangupButtonPressed;
		public event EventHandler OnBackButtonPressed;
		public event EventHandler OnHomeButtonPressed;
		public event EventHandler OnSearchButtonPressed;
		public event EventHandler OnManualDialButtonPressed;

		private readonly List<IVtcReferencedContactsView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcContactsView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
			m_ChildList = new List<IVtcReferencedContactsView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnTextEntered = null;
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
		public IEnumerable<IVtcReferencedContactsView> GetChildComponentViews(IViewFactory factory, ushort count)
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

		public void SetRecentButtonSelected(bool selected)
		{
			m_RecentsButton.SetSelected(selected);
		}

		public void SetCallButtonEnabled(bool enabled)
		{
			m_CallButton.Enable(enabled);
		}

		public void SetHangupButtonEnabled(bool enabled)
		{
			m_HangupButton.Enable(enabled);
		}

		public void SetSearchBarText(string text)
		{
			m_SearchBar.SetLabelText(text);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_SearchBar.OnTextModified += SearchBarOnTextModified;
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

			m_SearchBar.OnTextModified -= SearchBarOnTextModified;
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

		private void SearchBarOnTextModified(object sender, StringEventArgs stringEventArgs)
		{
			OnTextEntered.Raise(this, new StringEventArgs(stringEventArgs.Data));
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
