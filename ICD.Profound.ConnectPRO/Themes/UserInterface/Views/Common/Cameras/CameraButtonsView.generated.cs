using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Cameras
{
	public sealed partial class CameraButtonsView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_CameraConfigurationButtonList;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 130
			};

			m_CameraConfigurationButtonList = new VtProDynamicButtonList(9, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 3
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CameraConfigurationButtonList;
		}
	}
}
