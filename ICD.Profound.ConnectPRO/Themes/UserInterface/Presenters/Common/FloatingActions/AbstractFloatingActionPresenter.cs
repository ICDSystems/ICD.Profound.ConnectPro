using System;
using ICD.Common.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.FloatingActions
{
	public abstract class AbstractFloatingActionPresenter<TView> : AbstractUiPresenter<TView>, IFloatingActionPresenter<TView>
		where TView : class, IFloatingActionView
	{
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractFloatingActionPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(TView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool active = GetActive();
				bool enabled = GetEnabled();

				view.SetActive(active);
				view.SetEnabled(enabled);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Override to get the selected state for the button.
		/// </summary>
		/// <returns></returns>
		protected abstract bool GetActive();

		/// <summary>
		/// Override to get the enabled state for the button.
		/// </summary>
		/// <returns></returns>
		protected abstract bool GetEnabled();

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(TView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(TView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the option button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			if (GetEnabled())
				HandleButtonPress();
		}

		/// <summary>
		/// Override to handle the button press.
		/// </summary>
		protected abstract void HandleButtonPress();

		#endregion
	}
}
