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
	public partial class WtcJoinByIdView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleKeypad m_Keypad;
		private VtProTextEntry m_TextEntry;
		private VtProSimpleLabel m_MeetingIdLabel;
		private VtProButton m_JoinMyMeetingButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 740
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

			m_JoinMyMeetingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 723
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Keypad;
			yield return m_TextEntry;
			yield return m_MeetingIdLabel;
			yield return m_JoinMyMeetingButton;
		}
	}
}