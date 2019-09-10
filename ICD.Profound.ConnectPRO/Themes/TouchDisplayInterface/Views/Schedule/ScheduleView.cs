using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Schedule
{
	[ViewBinding(typeof(IScheduleView))]
    public sealed partial class ScheduleView : AbstractTouchDisplayView, IScheduleView
    {
        private readonly List<IReferencedBookingView> m_ChildList;

		public ScheduleView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		    m_ChildList = new List<IReferencedBookingView>();
		}

        public void SetAvailabilityText(string text)
        {
            m_RoomAvailabilityLabel.SetLabelText(text);
        }

        public void SetCurrentBookingIcon(string icon)
        {
            m_CurrentBookingIcon.SetIcon(icon);
        }

        public void SetCurrentBookingTime(string time)
        {
            m_CurrentBookingTimeLabel.SetLabelText(time);
        }

        public void SetCurrentBookingSubject(string meetingName)
        {
            m_CurrentBookingNameLabel.SetLabelText(meetingName);
        }

        /// <summary>
        /// Returns child views for list items.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<IReferencedBookingView> GetChildComponentViews(ITouchDisplayViewFactory factory, ushort count)
        {
            return GetChildViews(factory, m_ScheduleList, m_ChildList, count);
        }
    }
}
