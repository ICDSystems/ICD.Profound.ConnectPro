using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.EventArguments;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Contacts;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts
{
	[ViewBinding(typeof(IVtcContactsPolycomView))]
	public sealed partial class VtcContactsPolycomView : AbstractVtcContactsView, IVtcContactsPolycomView
	{
		/// <summary>
		/// Raised when the user presses the directory button.
		/// </summary>
		public event EventHandler OnNavigationButtonPressed;

		/// <summary>
		/// Raised when the user presses the favourites button.
		/// </summary>
		public event EventHandler OnLocalButtonPressed;

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
		public override event EventHandler OnDirectoryButtonPressed;

		/// <summary>
		/// Raised when the user presses the manual dial button.
		/// </summary>
		public override event EventHandler OnManualDialButtonPressed;

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
		public VtcContactsPolycomView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IVtcReferencedContactsView>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnNavigationButtonPressed = null;
			OnLocalButtonPressed = null;
			OnRecentButtonPressed = null;
			OnCallButtonPressed = null;
			OnHangupButtonPressed = null;
			OnBackButtonPressed = null;
			OnHomeButtonPressed = null;
			OnDirectoryButtonPressed = null;
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
		public override IEnumerable<IVtcReferencedContactsView> GetChildComponentViews(IViewFactory factory, ushort count)
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
			m_NavigationButton.SetSelected(selected);
		}

		public void SetLocalButtonSelected(bool selected)
		{
			m_LocalButton.SetSelected(selected);
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

		public void SetBackButtonVisible(bool visible)
		{
			m_BackButton.Show(visible);
		}

		public void SetHomeButtonVisible(bool visible)
		{
			m_HomeButton.Show(visible);
		}

		public void SetDirectoryButtonVisible(bool visible)
		{
			m_DirectoryButton.Show(visible);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_NavigationButton.OnPressed += NavigationButtonOnPressed;
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

			m_NavigationButton.OnPressed -= NavigationButtonOnPressed;
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

		private void NavigationButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnNavigationButtonPressed.Raise(this);
		}

		private void RecentsButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnRecentButtonPressed.Raise(this);
		}

		private void LocalButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnLocalButtonPressed.Raise(this);
		}

		private void ManualDialButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnManualDialButtonPressed.Raise(this);
		}

		private void DirectoryButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDirectoryButtonPressed.Raise(this);
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
