using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	public sealed partial class SettingsPasscodeView
	{
		private VtProSubpage m_VtProSubpage;
		private VtProFormattedText m_PasscodeLabel;
		private VtProSimpleKeypad m_Keypad;
		private VtProButton m_CancelButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_VtProSubpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 135
			};

			m_PasscodeLabel = new VtProFormattedText(panel, m_VtProSubpage);
			m_PasscodeLabel.SerialLabelJoins.Add(660);

			m_Keypad = new VtProSimpleKeypad(651, panel as IPanelDevice, m_VtProSubpage);

			m_CancelButton = new VtProButton(panel, m_VtProSubpage)
			{
				DigitalPressJoin = 660
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_VtProSubpage;
			yield return m_PasscodeLabel;
			yield return m_Keypad;
			yield return m_CancelButton;
		}
	}
}
