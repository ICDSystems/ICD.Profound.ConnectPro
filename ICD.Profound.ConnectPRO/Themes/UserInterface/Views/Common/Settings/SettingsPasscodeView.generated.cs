using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	public sealed partial class SettingsPasscodeView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_InstructionLabel;
		private VtProFormattedText m_PasscodeLabel;
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
				DigitalVisibilityJoin = 143
			};

			m_InstructionLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 801
			};

			m_PasscodeLabel = new VtProFormattedText(panel, m_Subpage);
			m_PasscodeLabel.SerialLabelJoins.Add(800);

			m_Keypad = new VtProSimpleKeypad(800, panel as IPanelDevice, m_Subpage);
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
