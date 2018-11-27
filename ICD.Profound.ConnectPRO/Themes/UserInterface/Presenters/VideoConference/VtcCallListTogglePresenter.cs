using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcCallListTogglePresenter : AbstractUiPresenter<IVtcCallListToggleView>, IVtcCallListTogglePresenter
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		public event EventHandler OnButtonPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private readonly IVtcKeyboardPresenter m_KeyboardPresenter;
		private readonly IVtcKeypadPresenter m_KeypadPresenter;

		private bool m_Mode;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcCallListTogglePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_KeyboardPresenter = nav.LazyLoadPresenter<IVtcKeyboardPresenter>();
			m_KeyboardPresenter.OnViewVisibilityChanged += KeyboardPresenterOnViewVisibilityChanged;

			m_KeypadPresenter = nav.LazyLoadPresenter<IVtcKeypadPresenter>();
			m_KeypadPresenter.OnViewVisibilityChanged += KeypadPresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonPressed = null;

			base.Dispose();

			m_KeyboardPresenter.OnViewVisibilityChanged -= KeyboardPresenterOnViewVisibilityChanged;
			m_KeypadPresenter.OnViewVisibilityChanged -= KeypadPresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcCallListToggleView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetContactsMode(m_Mode);

				bool hide = m_KeyboardPresenter.IsViewVisible || m_KeypadPresenter.IsViewVisible;

				view.SetButtonVisible(!hide);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// When true shows the "contacts" button, otherwise shows the "call" button.
		/// </summary>
		/// <param name="mode"></param>
		public void SetContactsMode(bool mode)
		{
			if (mode == m_Mode)
				return;

			m_Mode = mode;

			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the keypad visibility state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void KeypadPresenterOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		/// <summary>
		/// Called when the keyboard visibility state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void KeyboardPresenterOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			RefreshIfVisible();
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IVtcCallListToggleView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IVtcCallListToggleView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}
