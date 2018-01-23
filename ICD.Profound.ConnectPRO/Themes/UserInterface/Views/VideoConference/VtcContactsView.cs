using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcContactsView : AbstractView, IVtcContactsView
	{
		public event EventHandler<StringEventArgs> OnTextEntered;
		public event EventHandler OnDirectoryButtonPressed;
		public event EventHandler OnFavoritesButtonPressed;
		public event EventHandler OnRecentButtonPressed;
		public event EventHandler OnCallButtonPressed;
		public event EventHandler OnHangupButtonPressed;

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

		#endregion
	}
}
