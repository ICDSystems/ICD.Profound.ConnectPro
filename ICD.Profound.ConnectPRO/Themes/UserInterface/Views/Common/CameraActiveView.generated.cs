﻿using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public partial class CameraActiveView
	{
		private VtProSubpage m_Subpage;
		private VtProTabButton m_Tabs;
		private VtProDynamicButtonList m_CameraList;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 136
			};

			m_Tabs = new VtProTabButton(8, panel as IPanelDevice, m_Subpage)
			{
				DigitalVisibilityJoin = 637
			};

			m_CameraList = new VtProDynamicButtonList(17, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 10
			};
		}
		
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Tabs;
			yield return m_CameraList;
		}
	}
}
