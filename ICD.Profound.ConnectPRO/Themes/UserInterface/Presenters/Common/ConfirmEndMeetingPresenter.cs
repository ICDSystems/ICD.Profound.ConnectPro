using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(IConfirmEndMeetingPresenter))]
	public sealed class ConfirmEndMeetingPresenter : AbstractUiPresenter<IConfirmEndMeetingView>, IConfirmEndMeetingPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ConfirmEndMeetingPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		/// <summary>
		/// Called when the room enters/leaves meeting state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs eventArgs)
		{
			ShowView(false);
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IConfirmEndMeetingView view)
		{
			base.Subscribe(view);

			view.OnYesButtonPressed += ViewOnYesButtonPressed;
			view.OnCancelButtonPressed += ViewOnCancelButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IConfirmEndMeetingView view)
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
				Room.EndMeeting();
		}

		#endregion
	}
}
