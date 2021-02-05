using System;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(IConfirmLeaveCallPresenter))]
	public sealed class ConfirmLeaveCallPresenter : AbstractUiPresenter<IConfirmLeaveCallView>, IConfirmLeaveCallPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ConfirmLeaveCallPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IConfirmLeaveCallView view)
		{
			base.Subscribe(view);

			view.OnYesButtonPressed += ViewOnYesButtonPressed;
			view.OnCancelButtonPressed += ViewOnCancelButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IConfirmLeaveCallView view)
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
			Navigation.LazyLoadPresenter<IVtcBasePresenter>().ShowView(false);
			Navigation.LazyLoadPresenter<IWtcBasePresenter>().ShowView(false);

			ShowView(false);
		}

		#endregion
	}
}
