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
        private const string TIME_HTML_FORMAT = "<span style=\"color: {0}\">{1}</span>";
        private const string AVAILABLE_COLOR = "#67FCF1";
        private const string RESERVED_COLOR = "#F0544F";

        private readonly SafeCriticalSection m_RefreshSection;

        private bool m_Selected;

        public ReferencedBookingPresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views,
            ConnectProTheme theme)
            : base(nav, views, theme)
        {
            m_RefreshSection = new SafeCriticalSection();
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
                var color = Booking is EmptyBooking ? AVAILABLE_COLOR : RESERVED_COLOR;

                var timeString =
                    Booking.EndTime == DateTime.MaxValue
                        ? "Remaining Time"
                        : string.Format("{0} - {1}", FormatTime(Booking.StartTime), FormatTime(Booking.EndTime));

                // TODO: can't use css to change color on android
                view.SetTimeLabel(string.Format(TIME_HTML_FORMAT, color, timeString));

                view.SetSubjectLabel(Booking.IsPrivate ? "Private Meeting" : Booking.MeetingName);

                view.SetButtonEnabled(!(Booking is EmptyBooking));
                view.SetButtonSelected(m_Selected);
            }
            finally
            {
                m_RefreshSection.Leave();
            }
        }

        private static string FormatTime(DateTime time)
        {
            return ConnectProDateFormatting.GetShortTime(time);
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
            OnBookingPressed.Raise(this);
        }

        #endregion
    }
}