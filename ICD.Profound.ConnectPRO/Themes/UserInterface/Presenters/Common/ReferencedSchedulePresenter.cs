using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(IReferencedSchedulePresenter))]
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

		/// <summary>
		/// Sets the selected state.
		/// </summary>
		/// <value></value>
		public bool Selected
		{
			get { return m_Selected; }
			set
			{
				if (value == m_Selected)
					return;

				m_Selected = value;

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

			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		#region Methods

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
				bool allDay = (m_Booking.EndTime - m_Booking.StartTime).TotalHours >= 23;

				string startTime = allDay ? "All Day" : GetShortTime(m_Booking.StartTime);
				string endTime = allDay ? "All Day" : GetShortTime(m_Booking.EndTime);

				view.SetBookingIcon(m_Icon);
				view.SetStartTimeLabel(startTime);
				view.SetEndTimeLabel(endTime);
				view.SetSelected(m_Selected);
				view.SetPresenterNameLabel(m_Booking.OrganizerName);

				view.SetBodyLabel(m_Booking.IsPrivate ? "Private Meeting" : m_Booking.MeetingName);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#endregion

		#region Private Methods

		private string GetShortTime(DateTime time)
		{
			return Theme.DateFormatting.GetShortTime(time.ToLocalTime());
		}

		private string GetIconForBooking(IBooking booking)
		{
			if (booking == null)
				return null;

			IEnumerable<IConferenceDeviceControl> dialers =
				Room == null
					? Enumerable.Empty<IConferenceDeviceControl>()
					: Room.GetControlsRecursive<IConferenceDeviceControl>();

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

		#endregion

		#region Theme Callbacks

		private void DateFormattingOnFormatChanged(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		#endregion

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
