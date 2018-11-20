using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	public partial class WtcReferencedDirectoryItemView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_ContactButton;
		private VtProFormattedText m_ContactNameText;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_ContactButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};

			m_ContactNameText = new VtProFormattedText(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ContactButton;
			yield return m_ContactNameText;
		}
	}
}