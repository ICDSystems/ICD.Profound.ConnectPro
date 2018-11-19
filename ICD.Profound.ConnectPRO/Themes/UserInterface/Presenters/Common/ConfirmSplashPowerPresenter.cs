using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class ConfirmSplashPowerPresenter : AbstractUiPresenter<IConfirmSplashPowerView>,
	                                                  IConfirmSplashPowerPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ConfirmSplashPowerPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IConfirmSplashPowerView view)
		{
			base.Subscribe(view);

			view.OnYesButtonPressed += ViewOnYesButtonPressed;
			view.OnCancelButtonPressed += ViewOnCancelButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IConfirmSplashPowerView view)
		{
			base.Unsubscribe(view);

			view.OnYesButtonPressed -= ViewOnYesButtonPressed;
			view.OnCancelButtonPressed -= ViewOnCancelButtonPressed;
		}

		/// <summary>
		/// Called when a user presses the Cancel button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnCancelButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		/// <summary>
		/// Called when a user presses the Yes button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnYesButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Room.EndMeeting(true);
			ShowView(false);
		}

		#endregion
	}
}
