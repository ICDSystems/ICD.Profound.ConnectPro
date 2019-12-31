using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference
{
	public partial class ShareConferenceView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_SourceList;
		private VtProButton m_ShareButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 890
			};

			m_SourceList = new VtProDynamicButtonList(890, panel as IPanelDevice, m_Subpage);

			m_ShareButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 891,
				DigitalEnableJoin = 892,
				IndirectTextJoin = 891
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_SourceList;
			yield return m_ShareButton;
		}
	}
}
