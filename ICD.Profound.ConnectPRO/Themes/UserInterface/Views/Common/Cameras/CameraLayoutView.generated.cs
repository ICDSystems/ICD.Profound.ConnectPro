using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Cameras
{
	public sealed partial class CameraLayoutView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_SizeLayoutControl;
		private VtProDynamicButtonList m_StyleLayoutControl;
		private VtProDynamicButtonList m_ShareLayoutControl;
		private VtProDynamicButtonList m_SelfViewLayoutControl;
		private VtProDynamicButtonList m_PositionLayoutControl;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 350
			};

			m_SizeLayoutControl = new VtProDynamicButtonList(21, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 5,
				DigitalEnableJoin = 661
			};

			m_StyleLayoutControl = new VtProDynamicButtonList(22, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 4,
				DigitalEnableJoin = 662
			};

			m_ShareLayoutControl = new VtProDynamicButtonList(23, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 2,
				DigitalEnableJoin = 663
			};

			m_SelfViewLayoutControl = new VtProDynamicButtonList(24, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 2,
				DigitalEnableJoin = 664
			};

			m_PositionLayoutControl = new VtProDynamicButtonList(25, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 2,
				DigitalEnableJoin = 665
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_SizeLayoutControl;
			yield return m_StyleLayoutControl;
			yield return m_ShareLayoutControl;
			yield return m_SelfViewLayoutControl;
			yield return m_PositionLayoutControl;
		}
	}
}
