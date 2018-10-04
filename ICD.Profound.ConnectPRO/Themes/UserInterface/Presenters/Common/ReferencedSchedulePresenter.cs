using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Devices;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	public sealed class ReferencedSchedulePresenter : AbstractComponentPresenter<IReferencedScheduleView>,
														  IReferencedSchedulePresenter
	{
		/// <summary>
		/// Raised when the user presses the presenter.
		/// </summary>
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;
		private IBooking m_Booking;
		private bool m_Selected;

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
		public ReferencedSchedulePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
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
				string icon = GetIconForBooking(Booking, Room);

				view.SetBookingIcon(icon);
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

		private static string GetIconForBooking(IBooking booking, IConnectProRoom room)
		{
			if (booking == null)
				return null;

			switch (GetMeetingType(booking, room))
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

		private static eMeetingType GetMeetingType(IBooking booking, IConnectProRoom room)
		{
			// check if we have any dialers
			var dialers = room.GetControlsRecursive<IDialingDeviceControl>().ToList();
			if (dialers.Count == 0)
				return eMeetingType.Presentation;

			// Build map of dialer to best number
			Dictionary<IDialingDeviceControl, IBookingNumber> map = dialers.ToDictionary(d => d, d => GetBestNumber(d, booking));

			IDialingDeviceControl preferredDialer = dialers.Where(d => map.GetDefault(d) != null)
			                                               .OrderByDescending(d => d.CanDial(map[d]))
			                                               .ThenByDescending(d => d.Supports)
			                                               .FirstOrDefault();

			if (preferredDialer == null)
				return eMeetingType.Presentation;

			// route device to displays and/or audio destination
			var dialerDevice = preferredDialer.Parent;
			if (dialerDevice is IVideoConferenceDevice)
				return eMeetingType.VideoConference;

			if (preferredDialer.Supports == eConferenceSourceType.Audio)
				return eMeetingType.AudioConference;

			return eMeetingType.Presentation;
		}

		private static IBookingNumber GetBestNumber(IDialingDeviceControl dialer, IBooking booking)
		{
			if (dialer == null)
				throw new ArgumentNullException("dialer");

			if (booking == null)
				throw new ArgumentNullException("booking");

			return booking.GetBookingNumbers()
			              .OrderByDescending(n => dialer.CanDial(n))
			              .FirstOrDefault();
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
