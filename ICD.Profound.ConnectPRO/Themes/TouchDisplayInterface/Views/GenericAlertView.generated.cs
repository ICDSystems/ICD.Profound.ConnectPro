using System;
using System.Collections.Generic;
using System.Text;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	public partial class GenericAlertView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_AlertMessageLabel;
		private VtProButton m_DismissButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 7
			};

			m_AlertMessageLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 6
			};

			m_DismissButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 8,
				DigitalEnableJoin = 9
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_AlertMessageLabel;
			yield return m_DismissButton;
		}
	}
}
