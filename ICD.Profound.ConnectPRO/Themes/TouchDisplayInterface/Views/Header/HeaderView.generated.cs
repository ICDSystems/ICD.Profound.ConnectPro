using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.TextControls;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Header
{
	public partial class HeaderView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_RoomName;
		private VtProSimpleLabel m_TimeLabel;
        private VtProAdvancedButton m_CenterButton;
        private VtProDynamicIconObject m_CenterButtonIcon;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent);

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
                IndirectTextJoin = 11,
				
            };

            m_CenterButtonIcon = new VtProDynamicIconObject(panel, m_Subpage)
            {
	            DynamicIconSerialJoin = 12
            };
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RoomName;
			yield return m_TimeLabel;
            yield return m_CenterButton;
            yield return m_CenterButtonIcon;
		}
	}
}
