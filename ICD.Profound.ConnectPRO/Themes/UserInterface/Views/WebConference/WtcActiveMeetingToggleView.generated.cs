using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public partial class WtcActiveMeetingToggleView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ActiveMeetingButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 747
			};

			m_ActiveMeetingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 733,
				DigitalVisibilityJoin = 734
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ActiveMeetingButton;
		}
	}
}