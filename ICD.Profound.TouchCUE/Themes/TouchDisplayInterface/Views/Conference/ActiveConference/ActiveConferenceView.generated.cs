using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference.ActiveConference
{
	public partial class ActiveConferenceView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_ParticipantList;
		private VtProButton m_ShowHideCameraButton;
		private VtProButton m_LeaveMeetingButton;
		private VtProButton m_EndMeetingButton;
		private VtProSimpleLabel m_MeetingNumberLabel;
		private VtProSimpleLabel m_NoParticipantsLabel;
		private VtProButton m_InviteButton;
		private VtProButton m_LockButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 810
			};

			m_ShowHideCameraButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 811,
				DigitalLabelJoins = { 812 }
			};

			m_EndMeetingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 813,
				DigitalEnableJoin = 814
			};

			m_LeaveMeetingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 815,
				DigitalEnableJoin = 816
			};

			m_MeetingNumberLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 810,
				DigitalVisibilityJoin = 817
			};

			m_ParticipantList = new VtProSubpageReferenceList(810, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 50,
				DigitalJoinIncrement = 3,
				SerialJoinIncrement = 2
			};

			m_NoParticipantsLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 818
			};

			m_InviteButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 819,
				DigitalVisibilityJoin = 820
			};

			m_LockButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 821
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ParticipantList;
			yield return m_ShowHideCameraButton;
			yield return m_EndMeetingButton;
			yield return m_LeaveMeetingButton;
			yield return m_MeetingNumberLabel;
			yield return m_NoParticipantsLabel;
			yield return m_InviteButton;
			yield return m_LockButton;
		}
	}
}