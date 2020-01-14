using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;
using ICD.Connect.UI.Controls.Lists;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Schedule
{
    public sealed partial class ScheduleView
    {
        private VtProSubpage m_Subpage;

        private VtProDynamicIconObject m_CurrentBookingIcon;
        private VtProSimpleLabel m_CurrentBookingTimeLabel;
        private VtProSimpleLabel m_CurrentBookingNameLabel;
        private VtProSimpleLabel m_RoomAvailabilityLabel;
        private VtProAdvancedButton m_StartBookingButton;
		private VtProButton m_CloseBookingButton;

		private VtProSimpleLabel m_SchedulePage;
		private VtProSimpleLabel m_NoMeetingsPage;
        private VtProSubpageReferenceList m_ScheduleList;

        protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
        {
            m_Subpage = new VtProSubpage(panel, parent, index)
            {
                DigitalVisibilityJoin = 100
            };

            m_CurrentBookingIcon = new VtProDynamicIconObject(panel, m_Subpage)
            {
                DynamicIconSerialJoin = 102
            };

            m_CurrentBookingTimeLabel = new VtProSimpleLabel(panel, m_Subpage)
            {
                IndirectTextJoin = 103
            };

            m_CurrentBookingNameLabel = new VtProSimpleLabel(panel, m_Subpage)
            {
                IndirectTextJoin = 104
            };

            m_RoomAvailabilityLabel = new VtProSimpleLabel(panel, m_Subpage)
            {
                IndirectTextJoin = 105,
				DigitalVisibilityJoin = 105
            };

            m_StartBookingButton = new VtProAdvancedButton(panel, m_Subpage)
            {
                DigitalPressJoin = 101,
                DigitalVisibilityJoin = 102,
				IndirectTextJoin = 101,
				AnalogModeJoin = 101
            };

            m_CloseBookingButton = new VtProButton(panel, m_Subpage)
            {
	            DigitalPressJoin = 103,
	            DigitalVisibilityJoin = 104
            };

			
			m_SchedulePage = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 106
			};

			m_NoMeetingsPage = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 107
			};

            m_ScheduleList = new VtProSubpageReferenceList(5, panel as IPanelDevice, m_Subpage)
            {
                MaxSize = 100,
                SerialJoinIncrement = 2,
				DigitalJoinIncrement = 2
            };
        }

        protected override IEnumerable<IVtProControl> GetChildren()
        {
            yield return m_Subpage;
            yield return m_CurrentBookingIcon;
            yield return m_CurrentBookingTimeLabel;
            yield return m_CurrentBookingNameLabel;
            yield return m_RoomAvailabilityLabel;
            yield return m_StartBookingButton;
            yield return m_CloseBookingButton;
            yield return m_SchedulePage;
            yield return m_NoMeetingsPage;
            yield return m_ScheduleList;
        }
    }
}
