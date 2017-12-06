using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class EndMeetingPresenter : AbstractPresenter<IEndMeetingView>, IEndMeetingPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public EndMeetingPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
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
				Room.IsInMeeting = false;
		}

		#endregion
	}
}
