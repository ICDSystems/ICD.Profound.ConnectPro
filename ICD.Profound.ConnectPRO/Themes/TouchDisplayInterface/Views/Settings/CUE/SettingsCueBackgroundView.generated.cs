using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings.CUE
{
	public sealed partial class SettingsCueBackgroundView
	{
		private VtProSubpage m_Subpage;
		private VtProTabButton m_ModeButtons;
		private VtProButton m_EnabledToggleButton;

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
				DigitalVisibilityJoin = 1070
			};

			m_ModeButtons = new VtProTabButton(1070, panel as IPanelDevice, m_Subpage);

			m_EnabledToggleButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1071
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ModeButtons;
			yield return m_EnabledToggleButton;
		}
	}
}
