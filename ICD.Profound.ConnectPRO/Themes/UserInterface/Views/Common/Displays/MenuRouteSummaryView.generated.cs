using System;
using System.Collections.Generic;
using System.Text;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public partial class MenuRouteSummaryView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_RouteList;
		private VtProButton m_CloseButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 180
			};

			m_RouteList = new VtProSubpageReferenceList(20, panel as IPanelDevice, m_Subpage)
			{
				SerialJoinIncrement = 3,
				MaxSize = 12
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 181
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RouteList;
			yield return m_CloseButton;
		}
	}
}
