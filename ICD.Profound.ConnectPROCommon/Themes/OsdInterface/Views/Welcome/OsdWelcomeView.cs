using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Welcome;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Welcome
{
	[ViewBinding(typeof(IOsdWelcomeView))]
    public sealed partial class OsdWelcomeView : AbstractOsdView, IOsdWelcomeView
    {
        private readonly List<IReferencedScheduleView> m_ChildList;

		public OsdWelcomeView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		    m_ChildList = new List<IReferencedScheduleView>();
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
        public IEnumerable<IReferencedScheduleView> GetChildComponentViews(IOsdViewFactory factory, ushort count)
        {
            return GetChildViews(factory, m_ScheduleList, m_ChildList, count);
        }
    }
}
