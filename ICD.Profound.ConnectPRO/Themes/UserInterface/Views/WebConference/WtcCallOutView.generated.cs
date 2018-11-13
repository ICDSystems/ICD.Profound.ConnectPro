using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public partial class WtcCallOutView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleKeypad m_Keypad;
		private VtProTextEntry m_TextEntry;
		private VtProButton m_CallButton;
		private VtProButton m_HangupButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 744
			};

			m_Keypad = new VtProSimpleKeypad(702, panel as IPanelDevice, m_Subpage)
			{
				MiscButtonOneChar = '*',
				MiscButtonTwoChar = '#'
			};

			m_TextEntry = new VtProTextEntry(panel, m_Subpage)
			{
				IndirectTextJoin = 3031,
				SerialOutputJoin = 3031
			};

			m_CallButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 709,
				DigitalEnableJoin = 710
			};

			m_HangupButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 711,
				DigitalEnableJoin = 712
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Keypad;
			yield return m_TextEntry;
			yield return m_CallButton;
			yield return m_HangupButton;
		}
	}
}