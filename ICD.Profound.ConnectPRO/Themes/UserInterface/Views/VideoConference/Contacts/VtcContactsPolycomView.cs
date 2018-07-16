using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts
{
	public sealed partial class VtcContactsPolycomView : AbstractView, IVtcContactsPolycomView
	{
		/// <summary>
		/// Raised when the user presses the directory button.
		/// </summary>
		public event EventHandler OnDirectoryButtonPressed;

		/// <summary>
		/// Raised when the user presses the favourites button.
		/// </summary>
		public event EventHandler OnFavoritesButtonPressed;

		/// <summary>
		/// Raised when the user presses the recent button.
		/// </summary>
		public event EventHandler OnRecentButtonPressed;

		/// <summary>
		/// Raised when the user presses the call button.
		/// </summary>
		public event EventHandler OnCallButtonPressed;

		/// <summary>
		/// Raised when the user presses the hangup button.
		/// </summary>
		public event EventHandler OnHangupButtonPressed;

		/// <summary>
		/// Raised when the user presses the back button.
		/// </summary>
		public event EventHandler OnBackButtonPressed;

		/// <summary>
		/// Raised when the user presses the home button.
		/// </summary>
		public event EventHandler OnHomeButtonPressed;

		/// <summary>
		/// Raised when the user presses the search button.
		/// </summary>
		public event EventHandler OnSearchButtonPressed;

		/// <summary>
		/// Raised when the user presses the manual dial button.
		/// </summary>
		public event EventHandler OnManualDialButtonPressed;

		public event EventHandler OnDPadUpButtonPressed;
		public event EventHandler OnDPadDownButtonPressed;
		public event EventHandler OnDPadLeftButtonPressed;
		public event EventHandler OnDPadRightButtonPressed;
		public event EventHandler OnDPadSelectButtonPressed;

		private readonly List<IVtcReferencedContactsView> m_ChildList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public VtcContactsPolycomView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
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

			OnDPadUpButtonPressed = null;
			OnDPadDownButtonPressed = null;
			OnDPadLeftButtonPressed = null;
			OnDPadRightButtonPressed = null;
			OnDPadSelectButtonPressed = null;

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

		/// <summary>
		/// Sets the visibility of the DPad.
		/// </summary>
		/// <param name="visible"></param>
		public void SetDPadVisible(bool visible)
		{
			m_DPad.Show(visible);
			m_ContactList.Show(!visible);
		}

		public void SetDPadButtonSelected(bool selected)
		{
			m_DPadButton.SetSelected(selected);
		}

		public void SetLocalButtonSelected(bool selected)
		{
			m_LocalButton.SetSelected(selected);
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

		/// <summary>
		/// Sets the visibility of the navigation buttons.
		/// </summary>
		/// <param name="visible"></param>
		public void SetNavigationButtonsVisible(bool visible)
		{
			m_BackButton.Show(visible);
			m_HomeButton.Show(visible);

			// TODO
			m_DirectoryButton.Show(false);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DPadButton.OnPressed += DPadButtonOnPressed;
			m_LocalButton.OnPressed += LocalButtonOnPressed;
			m_RecentsButton.OnPressed += RecentsButtonOnPressed;
			m_CallButton.OnPressed += CallButtonOnPressed;
			m_HangupButton.OnPressed += HangupButtonOnPressed;
			m_BackButton.OnPressed += BackButtonOnPressed;
			m_HomeButton.OnPressed += HomeButtonOnPressed;
			m_DirectoryButton.OnPressed += DirectoryButtonOnPressed;
			m_ManualDialButton.OnPressed += ManualDialButtonOnPressed;
			m_DPad.OnButtonPressed += DPadOnButtonPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_DPadButton.OnPressed -= DPadButtonOnPressed;
			m_LocalButton.OnPressed -= LocalButtonOnPressed;
			m_RecentsButton.OnPressed -= RecentsButtonOnPressed;
			m_CallButton.OnPressed -= CallButtonOnPressed;
			m_HangupButton.OnPressed -= HangupButtonOnPressed;
			m_BackButton.OnPressed -= BackButtonOnPressed;
			m_HomeButton.OnPressed -= HomeButtonOnPressed;
			m_DirectoryButton.OnPressed -= DirectoryButtonOnPressed;
			m_ManualDialButton.OnPressed -= ManualDialButtonOnPressed;
			m_DPad.OnButtonPressed -= DPadOnButtonPressed;
		}

		private void DPadOnButtonPressed(object sender, DPadEventArgs dPadEventArgs)
		{
			switch (dPadEventArgs.Data)
			{
				case DPadEventArgs.eDirection.Up:
					OnDPadUpButtonPressed.Raise(this);
					break;

				case DPadEventArgs.eDirection.Down:
					OnDPadDownButtonPressed.Raise(this);
					break;

				case DPadEventArgs.eDirection.Left:
					OnDPadLeftButtonPressed.Raise(this);
					break;

				case DPadEventArgs.eDirection.Right:
					OnDPadRightButtonPressed.Raise(this);
					break;

				case DPadEventArgs.eDirection.Center:
					OnDPadSelectButtonPressed.Raise(this);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void HangupButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnHangupButtonPressed.Raise(this);
		}

		private void CallButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCallButtonPressed.Raise(this);
		}

		private void DPadButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDirectoryButtonPressed.Raise(this);
		}

		private void RecentsButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnRecentButtonPressed.Raise(this);
		}

		private void LocalButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnFavoritesButtonPressed.Raise(this);
		}

		private void ManualDialButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnManualDialButtonPressed.Raise(this);
		}

		private void DirectoryButtonOnPressed(object sender, EventArgs eventArgs)
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
