using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference
{
	public partial class CallOutConferenceView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleKeypad m_Keypad;
		private VtProFormattedText m_TextEntry;
		private VtProButton m_CallButton;
		private VtProButton m_BackButton;
		private VtProButton m_ClearButton;
		private VtProFormattedText m_CallStatus;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 950
			};
			
			m_Keypad = new VtProSimpleKeypad(950, panel as IPanelDevice, m_Subpage)
			{
				MiscButtonOneChar = '*',
				MiscButtonTwoChar = '#'
			};

			m_TextEntry = new VtProFormattedText(panel, m_Subpage)
			{
				IndirectTextJoin = 950
			};

			m_CallButton = new VtProButton(panel, m_Subpage)
			{
				IndirectTextJoin = 951,
				DigitalPressJoin = 951,
				DigitalEnableJoin = 952
			};

			m_BackButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 953,
				DigitalEnableJoin = 954
			};

			m_ClearButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 955,
				DigitalEnableJoin = 956
			};

			m_CallStatus = new VtProFormattedText(panel, m_Subpage);
			m_CallStatus.SerialLabelJoins.Add(952);
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Keypad;
			yield return m_TextEntry;
			yield return m_CallButton;
			yield return m_BackButton;
			yield return m_ClearButton;
			yield return m_CallStatus;
		}
	}
}