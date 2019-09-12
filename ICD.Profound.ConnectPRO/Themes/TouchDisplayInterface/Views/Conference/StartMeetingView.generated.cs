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
	public partial class StartMeetingView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_MeetNowButton;
		private VtProButton m_ContactsButton;
		private VtProButton m_JoinByIdButton;
		private VtProSimpleKeypad m_Keypad;
		private VtProTextEntry m_TextEntry;
		private VtProSimpleLabel m_MeetingIdLabel;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 738
			};

			m_MeetNowButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 724,
				DigitalEnableJoin = 748
			};

			m_JoinByIdButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 723,
				DigitalEnableJoin = 749
			};

			m_Keypad = new VtProSimpleKeypad(706, panel as IPanelDevice, m_Subpage);

			m_TextEntry = new VtProTextEntry(panel, m_Subpage)
			{
				IndirectTextJoin = 3032,
				SerialOutputJoin = 3032
			};

			m_MeetingIdLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 722
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_MeetNowButton;
			yield return m_JoinByIdButton;
			yield return m_Keypad;
			yield return m_TextEntry;
			yield return m_MeetingIdLabel;
		}
	}
}