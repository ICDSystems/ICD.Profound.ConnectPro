using System;
using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public partial class WtcMainPageView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_MeetNowButton;
		private VtProButton m_ContactsButton;
		private VtProButton m_JoinByIdButton;

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

			m_ContactsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 725
			};

			m_JoinByIdButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 726,
				DigitalEnableJoin = 749
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_MeetNowButton;
			yield return m_ContactsButton;
			yield return m_JoinByIdButton;
		}
	}
}