using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Bodies
{
    public sealed partial class OsdScheduleBodyView
	{
		private VtProSubpage m_Subpage;

	    private VtProDynamicIconObject m_CurrentBookingIcon;
	    private VtProSimpleLabel m_CurrentBookingTimeLabel;
	    private VtProSimpleLabel m_CurrentBookingNameLabel;
		private VtProSimpleLabel m_RoomAvailabilityLabel;

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
		        IndirectTextJoin = 105
		    };

			m_ScheduleList = new VtProSubpageReferenceList(5, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 6,
				SerialJoinIncrement = 2
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CurrentBookingIcon;
			yield return m_CurrentBookingTimeLabel;
			yield return m_CurrentBookingNameLabel;
			yield return m_RoomAvailabilityLabel;

			yield return m_ScheduleList;
		}
	}
}
