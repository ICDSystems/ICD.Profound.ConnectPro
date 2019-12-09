using System;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters
{
	[PresenterBinding(typeof(IGenericAlertPresenter))]
	public sealed class GenericAlertPresenter : AbstractTouchDisplayPresenter<IGenericAlertView>, IGenericAlertPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_CloseTimer;

		private string Message { get; set; }

		private bool Timed { get; set; }

		public GenericAlertPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_CloseTimer = SafeTimer.Stopped(() => ShowView(false));
		}

		protected override void Refresh(IGenericAlertView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetAlertText(Message);
				view.SetDismissButtonEnabled(!Timed);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void Show(string message)
		{
			Timed = false;
			Message = message;
			Refresh();
			ShowView(true);
		}

		public void Show(string message, long time)
		{
			Timed = true;
			m_CloseTimer.Reset(time);
			Message = message;
			Refresh();
			ShowView(true);
		}

		#region Room Callbacks

		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnOnIsInMeetingChanged;
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

			room.OnIsInMeetingChanged -= RoomOnOnIsInMeetingChanged;
		}

		private void RoomOnOnIsInMeetingChanged(object sender, BoolEventArgs e)
		{
			if (IsViewVisible && !e.Data)
				ShowView(false);
		}

		#endregion

		#region View Callbacks

		protected override void Subscribe(IGenericAlertView view)
		{
			base.Subscribe(view);

			view.OnDismissButtonPressed += ViewOnDismissButtonPressed;
		}

		protected override void Unsubscribe(IGenericAlertView view)
		{
			base.Unsubscribe(view);

			view.OnDismissButtonPressed -= ViewOnDismissButtonPressed;
		}

		private void ViewOnDismissButtonPressed(object sender, EventArgs e)
		{
			ShowView(false);
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
				return;

			m_CloseTimer.Stop();
			Timed = false;
		}

		#endregion
	}
}