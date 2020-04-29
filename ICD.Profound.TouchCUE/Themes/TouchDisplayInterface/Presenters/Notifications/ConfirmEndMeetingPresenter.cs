using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Notifications;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters.Notifications
{
	[PresenterBinding(typeof(IConfirmEndMeetingPresenter))]
	public sealed class ConfirmEndMeetingPresenter : AbstractTouchDisplayPresenter<IConfirmEndMeetingView>, IConfirmEndMeetingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private string m_ConfirmText;
		private Action m_ConfirmAction;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ConfirmEndMeetingPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, TouchCueTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		public void Show(string text, Action confirmAction)
		{
			m_ConfirmText = text;
			m_ConfirmAction = confirmAction;
			ShowView(true);
		}

		protected override void Refresh(IConfirmEndMeetingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetConfirmText(m_ConfirmText);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
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
			if (m_ConfirmAction != null)
				m_ConfirmAction();

			ShowView(false);
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (!args.Data)
			{
				m_ConfirmAction = null;
				m_ConfirmText = string.Empty;
			}
		}

		#endregion
	}
}
