using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public partial class WtcLeftMenuView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_ButtonList;
		private VtProAdvancedButton m_ActiveMeetingIndicator;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 15999
			};

			m_ButtonList = new VtProDynamicButtonList(703, panel as IPanelDevice, m_Subpage);

			m_ActiveMeetingIndicator = new VtProAdvancedButton(panel, m_Subpage)
			{
				AnalogModeJoin = 701
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ButtonList;
			yield return m_ActiveMeetingIndicator;
		}
	}
}