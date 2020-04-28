using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Calendaring.Booking;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.Shared.Models;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Schedule;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Schedule
{
	[PresenterBinding(typeof(IReferencedBookingPresenter))]
	public sealed class ReferencedBookingPresenter : AbstractTouchDisplayComponentPresenter<IReferencedBookingView>,
		IReferencedBookingPresenter
	{
		private const string HTML_COLOR_FORMAT = "<span style=\"color: {0}\">{1}</span>";
		private const string AVAILABLE_COLOR = "#67FCF1";
		private const string RESERVED_TIME_COLOR = "#F0544F";
		private const string RESERVED_NAME_COLOR = "#227385";
		private const string SELECTED_COLOR = "#FFFFFF";

		private readonly SafeCriticalSection m_RefreshSection;

		private bool m_Selected;

		public ReferencedBookingPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
			ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			theme.DateFormatting.OnFormatChanged += DateFormattingOnFormatChanged;
		}

		public event EventHandler OnBookingPressed;

		public IBooking Booking { get; set; }

		public void SetSelected(bool selected)
		{
			m_Selected = selected;
		}

		protected override void Refresh(IReferencedBookingView view)
		{
			base.Refresh(view);

			if (Booking == null)
				return;

			m_RefreshSection.Enter();

			try
			{

				var timeColor = !m_Selected ? (Booking is EmptyBooking ? AVAILABLE_COLOR : RESERVED_TIME_COLOR) : SELECTED_COLOR;
				var nameColor = m_Selected || Booking is EmptyBooking ? SELECTED_COLOR : RESERVED_NAME_COLOR;

				var timeString =
					Booking.EndTime == DateTime.MaxValue
						? "Remaining Time"
						: string.Format("{0} - {1}", FormatTime(Booking.StartTime), FormatTime(Booking.EndTime));

				// TODO: can't use css to change color on android
				view.SetTimeLabel(string.Format(HTML_COLOR_FORMAT, timeColor, timeString));

				var subjectString = Booking.IsPrivate ? "Private Meeting" : Booking.MeetingName;
				view.SetSubjectLabel(string.Format(HTML_COLOR_FORMAT, nameColor, subjectString));

				view.SetButtonEnabled(!(Booking is EmptyBooking));
				view.SetButtonSelected(m_Selected);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private string FormatTime(DateTime time)
		{
			return Theme.DateFormatting.GetShortTime(time);
		}

		private void DateFormattingOnFormatChanged(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#region View Callbacks

		protected override void Subscribe(IReferencedBookingView view)
		{
			base.Subscribe(view);

			view.OnPressed += ViewOnPressed;
		}

		protected override void Unsubscribe(IReferencedBookingView view)
		{
			base.Unsubscribe(view);

			view.OnPressed -= ViewOnPressed;
		}

		private void ViewOnPressed(object sender, EventArgs e)
		{
			if (Booking is EmptyBooking)
				return;
			OnBookingPressed.Raise(this);
		}

		#endregion
	}
}