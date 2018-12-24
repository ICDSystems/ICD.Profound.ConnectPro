using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	public partial class WtcActiveMeetingView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_ParticipantList;
		private VtProButton m_ShowHideCameraButton;
		private VtProButton m_LeaveMeetingButton;
		private VtProButton m_EndMeetingButton;
		private VtProButton m_MeetingInfoButton;
		private VtProSimpleLabel m_NoParticipantsLabel;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 741
			};

			m_ShowHideCameraButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 708,
				DigitalLabelJoins = { 758 }
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

			m_MeetingInfoButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 759,
				DigitalEnableJoin = 760
			};

			m_ParticipantList = new VtProSubpageReferenceList(700, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 20,
				DigitalJoinIncrement = 3,
				SerialJoinIncrement = 2
			};

			m_NoParticipantsLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 757
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ParticipantList;
			yield return m_ShowHideCameraButton;
			yield return m_EndMeetingButton;
			yield return m_LeaveMeetingButton;
			yield return m_MeetingInfoButton;
			yield return m_NoParticipantsLabel;
		}
	}
}