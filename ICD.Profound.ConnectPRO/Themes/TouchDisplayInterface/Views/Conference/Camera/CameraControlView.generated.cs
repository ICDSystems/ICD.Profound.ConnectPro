using System.Collections.Generic;
using ICD.Common.Utils.Collections;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference.Camera
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
		private VtProButton m_PresetButton6;
		private VtProSimpleLabel m_PresetButtonPage;

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
				DigitalVisibilityJoin = 960
			};

			m_DPad = new VtProDPad(960, panel as IPanelDevice, m_Subpage)
			{
				//DigitalEnableJoin = 625
			};

			m_ZoomInButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 961,
				//DigitalEnableJoin = 624
			};

			m_ZoomOutButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 962,
				//DigitalEnableJoin = 624
			};

			m_PresetButton1 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 963,
				//DigitalEnableJoin = 626,
				IndirectTextJoin = 963,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton2 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 964,
				//DigitalEnableJoin = 626,
				IndirectTextJoin = 964,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton3 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 965,
				//DigitalEnableJoin = 626,
				IndirectTextJoin = 965,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton4 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 966,
				//DigitalEnableJoin = 626,
				IndirectTextJoin = 966,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton5 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 967,
				//DigitalEnableJoin = 626,
				IndirectTextJoin = 967,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButton6 = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 968,
				//DigitalEnableJoin = 626,
				IndirectTextJoin = 968,
				HoldDuration = PRESET_HOLD_MILLISECONDS
			};

			m_PresetButtonPage = new VtProSimpleLabel(Panel, m_Subpage)
			{
				DigitalVisibilityJoin = 969
			};

			m_PresetButtons = new BiDictionary<ushort, VtProButton>
			{
				{0, m_PresetButton1},
				{1, m_PresetButton2},
				{2, m_PresetButton3},
				{3, m_PresetButton4},
				{4, m_PresetButton5},
				{5, m_PresetButton6}
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
			yield return m_PresetButton6;
			yield return m_PresetButtonPage;
		}
	}
}
