using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference.Camera
{
	public sealed partial class CameraLayoutView
	{
		private VtProSubpage m_Subpage;
		
		private VtProButton m_ContentThumbnailButton;
		private VtProButton m_SelfviewCameraButton;

		private VtProButton m_SizeOffButton;
		private VtProButton m_Size1Button;
		private VtProButton m_Size2Button;
		private VtProButton m_Size3Button;
		
		private VtProButton m_GalleryButton;
		private VtProButton m_SpeakerButton;
		private VtProButton m_StripButton;
		private VtProButton m_ShareAllButton;
		
		private VtProButton m_TopLeftButton;
		private VtProButton m_TopRightButton;
		private VtProButton m_BottomLeftButton;
		private VtProButton m_BottomRightButton;

		private Dictionary<ushort, VtProButton> m_LayoutStyles;
		private Dictionary<ushort, VtProButton> m_LayoutSizes;
		private Dictionary<ushort, VtProButton> m_LayoutPositions;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 900
			};

			m_ContentThumbnailButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 901,
				DigitalEnableJoin = 902
			};

			m_SelfviewCameraButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 903,
				DigitalEnableJoin = 904
			};

			m_SizeOffButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 905,
				DigitalEnableJoin = 906
			};
			m_Size1Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 907,
				DigitalEnableJoin = 908
			};
			m_Size2Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 909,
				DigitalEnableJoin = 910
			};
			m_Size3Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 911,
				DigitalEnableJoin = 912
			};
			
			m_GalleryButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 913,
				DigitalEnableJoin = 914
			};
			m_SpeakerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 915,
				DigitalEnableJoin = 916
			};
			m_StripButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 917,
				DigitalEnableJoin = 918
			};
			m_ShareAllButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 919,
				DigitalEnableJoin = 920
			};
			
			m_TopLeftButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 921,
				DigitalVisibilityJoin = 922

			};
			m_TopRightButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 923,
				DigitalVisibilityJoin = 924
			};
			m_BottomLeftButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 925,
				DigitalVisibilityJoin = 926
			};
			m_BottomRightButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 927,
				DigitalVisibilityJoin = 928
			};

			m_LayoutStyles =
				new Dictionary<ushort, VtProButton>
				{
					{0, m_GalleryButton},
					{1, m_SpeakerButton},
					{2, m_StripButton},
					{3, m_ShareAllButton}
				};

			m_LayoutSizes =
				new Dictionary<ushort, VtProButton>
				{
					{0, m_SizeOffButton},
					{1, m_Size1Button},
					{2, m_Size2Button},
					{3, m_Size3Button}
				};

			m_LayoutPositions =
				new Dictionary<ushort, VtProButton>
				{
					{0, m_TopLeftButton},
					{1, m_TopRightButton},
					{2, m_BottomLeftButton},
					{3, m_BottomRightButton}
				};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ContentThumbnailButton;
			yield return m_SelfviewCameraButton;
		}
	}
}
