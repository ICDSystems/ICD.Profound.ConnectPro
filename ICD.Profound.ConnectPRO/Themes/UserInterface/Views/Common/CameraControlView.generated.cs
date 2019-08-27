using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class CameraControlView
	{
		private const long PRESET_HOLD_MILLISECONDS = 1000;

		private VtProSubpage m_Subpage;
		private VtProDPad m_DPad;
		private VtProButton m_ZoomInButton;
		private VtProButton m_ZoomOutButton;
		private VtProButton m_PresetButton1;
		private VtProButton m_PresetButton2;
		private VtProButton m_PresetButton3;
		private VtProButton m_PresetButton4;
		private VtProButton m_PresetButton5;
		private VtProSimpleLabel m_PresetStoredLabel;
		private VtProDynamicButtonList m_CameraList;
		private VtProTabButton m_Tabs;

		private BiDictionary<ushort, VtProButton> m_PresetButtons;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 137
			};

			m_DPad = new VtProDPad(620, panel as IPanelDevice, m_Subpage);

			m_ZoomInButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 621
			};

			m_ZoomOutButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 622
			};

			m_PresetButton1 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 631,
				IndirectTextJoin = 631,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton2 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 632,
				IndirectTextJoin = 632,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton3 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 633,
				IndirectTextJoin = 633,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton4 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 634,
				IndirectTextJoin = 634,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton5 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 635,
				IndirectTextJoin = 635,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetStoredLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 636
			};

			m_PresetButtons = new BiDictionary<ushort, VtProButton>
			{
				{0, m_PresetButton1},
				{1, m_PresetButton2},
				{2, m_PresetButton3},
				{3, m_PresetButton4},
				{4, m_PresetButton5}
			};

			m_CameraList = new VtProDynamicButtonList(16, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 10
			};

			m_Tabs = new VtProTabButton(9, panel as IPanelDevice, m_Subpage)
			{
				DigitalVisibilityJoin = 638
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_DPad;
			yield return m_ZoomInButton;
			yield return m_ZoomOutButton;
			yield return m_PresetButton1;
			yield return m_PresetButton2;
			yield return m_PresetButton3;
			yield return m_PresetButton4;
			yield return m_PresetButton5;
			yield return m_PresetStoredLabel;
			yield return m_CameraList;
			yield return m_Tabs;
		}
	}
}
