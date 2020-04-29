using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Settings.Administrative
{
	public sealed partial class SettingsPinView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_InstructionLabel;
		private VtProDynamicButtonList m_PasscodeLabel;
		private VtProSimpleKeypad m_Keypad;

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
				DigitalVisibilityJoin = 1040
			};

			m_InstructionLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1040
			};

			m_PasscodeLabel = new VtProDynamicButtonList(1040, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 4
			};

			m_Keypad = new VtProSimpleKeypad(1041, panel as IPanelDevice, m_Subpage);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_InstructionLabel;
			yield return m_PasscodeLabel;
			yield return m_Keypad;
		}
	}
}
