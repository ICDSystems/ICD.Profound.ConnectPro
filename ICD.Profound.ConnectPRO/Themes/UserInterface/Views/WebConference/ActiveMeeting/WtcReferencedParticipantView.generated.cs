using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	public partial class WtcReferencedParticipantView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ParticipantButton;
		private VtProFormattedText m_ParticipantNameText;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_ParticipantButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};

			m_ParticipantNameText = new VtProFormattedText(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ParticipantButton;
			yield return m_ParticipantNameText;
		}
	}
}