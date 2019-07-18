using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public partial class Menu3PlusDisplaysView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_DisplayList;
		
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 116
			};

			m_DisplayList = new VtProSubpageReferenceList(7, panel as IPanelDevice, m_Subpage)
			{
				AnalogJoinIncrement = 1,
				DigitalVisibilityJoin = 3,
				SerialJoinIncrement = 4,
				MaxSize = 100
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_DisplayList;
		}
	}
}
