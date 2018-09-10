using System;
using ICD.Common.Utils.Extensions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public sealed class VtcCallListTogglePresenter : AbstractPresenter<IVtcCallListToggleView>, IVtcCallListTogglePresenter
	{
		/// <summary>
		/// Raised when the user presses the button.
		/// </summary>
		public event EventHandler OnButtonPressed;

		private bool m_Mode;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public VtcCallListTogglePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IVtcCallListToggleView view)
		{
			base.Refresh(view);

			view.SetContactsMode(m_Mode);
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
