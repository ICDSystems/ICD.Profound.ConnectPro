using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class ReferencedSchedulePresenter : AbstractUiComponentPresenter<IReferencedScheduleView>,
														  IReferencedSchedulePresenter
	{
		/// <summary>
		/// Raised when the user presses the presenter.
		/// </summary>
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private IBooking m_Booking;
		private bool m_Selected;
		private string m_Icon;

		#region Properties

		/// <summary>
		/// Gets/sets the source for the presenter.
		/// </summary>
		[CanBeNull]
		public IBooking Booking
		{
			get { return m_Booking; }
			set
			{
				if (value == m_Booking)
					return;

				m_Booking = value;
				m_Icon = GetIconForBooking(m_Booking);

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedSchedulePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		public void SetSelected(bool selected)
		{
			m_Selected = selected;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedScheduleView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				view.SetBookingIcon(m_Icon);
                view.SetStartTimeLabel(m_Booking.StartTime.ToShortTimeString());
			    view.SetEndTimeLabel(m_Booking.EndTime.ToShortTimeString());
			    view.SetSelected(m_Selected);
			    view.SetPresenterNameLabel(m_Booking.OrganizerName);

			    view.SetBodyLabel(m_Booking.IsPrivate ? "Private Meeting" : m_Booking.MeetingName);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private string GetIconForBooking(IBooking booking)
		{
			if (booking == null)
				return null;

			IEnumerable<IConferenceDeviceControl> dialers =
				Room == null ? Enumerable.Empty<IConferenceDeviceControl>() : Room.GetControlsRecursive<IConferenceDeviceControl>();

			switch (ConferencingBookingUtils.GetMeetingType(booking, dialers))
			{
				case eMeetingType.AudioConference:
					return Icons.GetSourceIcon("audioConference", eSourceColor.Grey);
				case eMeetingType.VideoConference:
					return Icons.GetSourceIcon("videoConference", eSourceColor.Grey);
				case eMeetingType.Presentation:
					return Icons.GetSourceIcon("display", eSourceColor.Grey);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IReferencedScheduleView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedScheduleView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the source button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
