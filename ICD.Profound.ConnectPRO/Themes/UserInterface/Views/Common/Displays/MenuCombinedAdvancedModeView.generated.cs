using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public partial class MenuCombinedAdvancedModeView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_DisplaysList;
		private VtProButton m_SimpleModeButton;
		private VtProButton m_RouteSummaryButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 111
			};

			m_DisplaysList = new VtProSubpageReferenceList(6, panel as IPanelDevice, m_Subpage)
			{
				DigitalJoinIncrement = 3,
				SerialJoinIncrement = 4
			};

			m_SimpleModeButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 690,
				DigitalEnableJoin = 691
			};

			m_RouteSummaryButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 692,
				DigitalEnableJoin = 693
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_DisplaysList;
			yield return m_SimpleModeButton;
			yield return m_RouteSummaryButton;
		}
	}
}
