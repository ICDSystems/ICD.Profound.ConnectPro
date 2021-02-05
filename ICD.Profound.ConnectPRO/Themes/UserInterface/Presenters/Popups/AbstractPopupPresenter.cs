using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups
{
	public abstract class AbstractPopupPresenter<TView> : AbstractUiPresenter<TView>, IPopupPresenter<TView>
		where TView : class, IPopupView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractPopupPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region Methods

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(TView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(TView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		#endregion
	}
}
