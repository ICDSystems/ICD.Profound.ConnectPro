using System;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(IEndMeetingPresenter))]
	public sealed class EndMeetingPresenter : AbstractUiPresenter<IEndMeetingView>, IEndMeetingPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public EndMeetingPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IEndMeetingView view)
		{
			base.Subscribe(view);

			view.OnEndMeetingButtonPressed += ViewOnEndMeetingButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IEndMeetingView view)
		{
			base.Unsubscribe(view);

			view.OnEndMeetingButtonPressed -= ViewOnEndMeetingButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the end meeting button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnEndMeetingButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room != null)
				Navigation.NavigateTo<IConfirmEndMeetingPresenter>();
		}

		#endregion
	}
}
