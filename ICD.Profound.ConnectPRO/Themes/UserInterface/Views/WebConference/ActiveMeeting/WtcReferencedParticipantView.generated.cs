using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	public partial class WtcReferencedParticipantView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ParticipantButton;
		private VtProFormattedText m_ParticipantNameText;
		private VtProImageObject m_AvatarImage;
		private VtProDynamicIconObject m_MuteIcon;

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

			m_AvatarImage = new VtProImageObject(panel, m_Subpage)
			{
				SerialGraphicsJoin = 2,
				DigitalVisibilityJoin = 2
			};

			m_MuteIcon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 3
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ParticipantButton;
			yield return m_ParticipantNameText;
			yield return m_AvatarImage;
			yield return m_MuteIcon;
		}
	}
}