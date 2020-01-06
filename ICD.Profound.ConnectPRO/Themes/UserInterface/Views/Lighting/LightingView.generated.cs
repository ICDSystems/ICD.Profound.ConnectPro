using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Lighting
{
	public sealed partial class LightingView
	{
		private const int MAX_PRESETS = 5;

		private VtProSubpage m_Subpage;
		private VtProButton m_CloseButton;
		private VtProDynamicButtonList m_PresetButtonList;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 1200
			};

			m_CloseButton = new VtProButton(panel, parent)
			{
				DigitalPressJoin = 1201
			};

			m_PresetButtonList = new VtProDynamicButtonList(120, panel as IPanelDevice, parent)
			{
				MaxSize =  MAX_PRESETS
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CloseButton;
			yield return m_PresetButtonList;
		}
	}
}
