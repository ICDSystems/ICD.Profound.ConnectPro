using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Background
{
	public partial class BackgroundView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_Background;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent);

			m_Background = new VtProAdvancedButton(panel, m_Subpage)
			{
				AnalogModeJoin = 15,
				DigitalEnableJoin = 15
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Background;
		}
	}
}
