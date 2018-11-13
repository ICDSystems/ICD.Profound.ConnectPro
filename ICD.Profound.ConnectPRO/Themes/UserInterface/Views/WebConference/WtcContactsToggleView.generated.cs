using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public partial class WtcContactsToggleView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ContactsButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 746
			};

			m_ContactsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 735,
				DigitalVisibilityJoin = 736
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ContactsButton;
		}
	}
}