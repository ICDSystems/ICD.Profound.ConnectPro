using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	public partial class WtcActiveMeetingView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_ParticipantList;
		private VtProButton m_MuteParticipantButton;
		private VtProButton m_KickParticipantButton;
		private VtProButton m_ShowHideCameraButton;
		private VtProButton m_LeaveMeetingButton;
		private VtProButton m_EndMeetingButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 741
			};

			m_KickParticipantButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 706,
				DigitalEnableJoin = 750
			};

			m_MuteParticipantButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 707,
				DigitalEnableJoin = 751
			};

			m_ShowHideCameraButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 708
			};

			m_EndMeetingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 702,
				DigitalEnableJoin = 703
			};

			m_LeaveMeetingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 704,
				DigitalEnableJoin = 705
			};

			m_ParticipantList = new VtProSubpageReferenceList(700, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 20,
				DigitalJoinIncrement = 3,
				SerialJoinIncrement = 1
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ParticipantList;
			yield return m_KickParticipantButton;
			yield return m_MuteParticipantButton;
			yield return m_ShowHideCameraButton;
			yield return m_EndMeetingButton;
			yield return m_LeaveMeetingButton;
		}
	}
}