using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	public partial class WtcContactListView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_ContactList;
		private VtProSubpageReferenceList m_SelectedContactList;
		private VtProButton m_SearchButton;
		private VtProButton m_InviteParticipantButton;
		
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 739
			};

			m_ContactList = new VtProSubpageReferenceList(705, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 20,
				DigitalJoinIncrement = 3,
				AnalogJoinIncrement = 1,
				SerialJoinIncrement = 2
			};

			m_SelectedContactList = new VtProSubpageReferenceList(708, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 20,
				DigitalJoinIncrement = 2,
				AnalogJoinIncrement = 0,
				SerialJoinIncrement = 2
			};

			m_SearchButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 752,
				DigitalEnableJoin = 753
			};

			m_InviteParticipantButton = new VtProButton(panel, m_Subpage)
			{
                DigitalPressJoin = 721,
				DigitalEnableJoin = 754
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ContactList;
			yield return m_SelectedContactList;
			yield return m_InviteParticipantButton;
			yield return m_SearchButton;
		}
	}
}