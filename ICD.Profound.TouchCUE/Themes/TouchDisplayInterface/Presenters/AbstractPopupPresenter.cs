using System;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters
{
	public abstract class AbstractPopupPresenter<TView> : AbstractTouchDisplayPresenter<TView>, IPopupPresenter<TView>
		where TView : class, IPopupView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractPopupPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
		}

		#region Methods

		/// <summary>
		/// Closes the popup.
		/// </summary>
		public virtual void Close()
		{
			ShowView(false);
		}

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
			Close();
		}

		#endregion
	}
}
