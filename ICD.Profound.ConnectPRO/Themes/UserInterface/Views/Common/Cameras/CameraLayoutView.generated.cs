using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Cameras
{
	public sealed partial class CameraLayoutView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_LayoutSizeButtonList;
		private VtProDynamicButtonList m_LayoutStyleButtonList;
		private VtProButton m_ContentThumbnailButton;
		private VtProButton m_SelfviewCameraButton;
		private VtProDynamicButtonList m_ThumbnailPositionButtonList;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 350
			};

			m_LayoutSizeButtonList = new VtProDynamicButtonList(21, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 4,
				DigitalEnableJoin = 661
			};

			m_LayoutStyleButtonList = new VtProDynamicButtonList(22, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 4,
				DigitalEnableJoin = 662
			};

			m_ContentThumbnailButton = new VtProButton(panel, m_Subpage)
			{
				DigitalEnableJoin = 663,
				DigitalPressJoin = 666
			};

			m_SelfviewCameraButton = new VtProButton(panel, m_Subpage)
			{
				DigitalEnableJoin = 664,
				DigitalPressJoin = 667
			};

			m_ThumbnailPositionButtonList = new VtProDynamicButtonList(25, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 2,
				DigitalEnableJoin = 665
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_LayoutSizeButtonList;
			yield return m_LayoutStyleButtonList;
			yield return m_ContentThumbnailButton;
			yield return m_SelfviewCameraButton;
			yield return m_ThumbnailPositionButtonList;
		}
	}
}
