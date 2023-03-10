using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Header
{
	public partial class HeaderView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_RoomName;
		private VtProSimpleLabel m_TimeLabel;
        private VtProAdvancedButton m_CenterButton;
        private VtProButton m_CollapseButton;
        private VtProSubpageReferenceList m_LeftButtonList;
        private VtProSubpageReferenceList m_RightButtonList;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 50
			};

			m_RoomName = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 10
			};

			m_TimeLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 9
			};

            m_CenterButton = new VtProAdvancedButton(panel, m_Subpage)
            {
                DigitalPressJoin = 11,
				DigitalEnableJoin = 12,
                IndirectTextJoin = 11,
				AnalogModeJoin = 11
            };

            m_CollapseButton = new VtProButton(panel, m_Subpage)
            {
	            DigitalPressJoin = 13
            };

            m_LeftButtonList = new VtProSubpageReferenceList(11, panel as IPanelDevice, m_Subpage)
            {
				AnalogJoinIncrement = 1,
				DigitalJoinIncrement = 3,
				SerialJoinIncrement = 2,
				MaxSize = 10
            };

            m_RightButtonList = new VtProSubpageReferenceList(12, panel as IPanelDevice, m_Subpage)
            {
	            AnalogJoinIncrement = 1,
	            DigitalJoinIncrement = 3,
	            SerialJoinIncrement = 2,
	            MaxSize = 10
            };
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RoomName;
			yield return m_TimeLabel;
            yield return m_CenterButton;
            yield return m_CollapseButton;
            yield return m_LeftButtonList;
            yield return m_RightButtonList;
		}
	}
}
