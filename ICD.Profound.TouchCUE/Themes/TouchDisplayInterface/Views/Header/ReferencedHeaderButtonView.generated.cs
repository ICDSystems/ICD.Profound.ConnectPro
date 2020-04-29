using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Header
{
	public partial class ReferencedHeaderButtonView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_Button;
		private VtProDynamicIconObject m_Icon;
		private VtProDynamicIconObject m_Background; //not an icon, just need visibility

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_Button = new VtProAdvancedButton(panel, m_Subpage)
			{
				AnalogModeJoin = 1,
				DigitalPressJoin = 1,
				DigitalEnableJoin = 2,
				IndirectTextJoin = 2
			};

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 1
			};

			m_Background = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 3
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Button;
			yield return m_Icon;
			yield return m_Background;
		}
	}
}