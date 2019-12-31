using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.DeviceDrawer
{
	public sealed partial class DeviceDrawerView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_SourceList;
		private VtProDynamicButtonList m_AppButtonList;
		
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 200
			};

			m_SourceList = new VtProSubpageReferenceList(6, panel as IPanelDevice, m_Subpage)
			{
				DigitalJoinIncrement = 2,
				SerialJoinIncrement = 3,
				AnalogJoinIncrement = 1,
				MaxSize = 100
			};

			m_AppButtonList = new VtProDynamicButtonList(201, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 6
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_SourceList;
			yield return m_AppButtonList;
		}
	}
}
