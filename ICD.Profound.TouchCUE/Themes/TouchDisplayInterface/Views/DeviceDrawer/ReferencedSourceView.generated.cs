using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.DeviceDrawer
{
	public partial class ReferencedSourceView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicIconObject m_Icon;
		private VtProSimpleLabel m_Name;
		private VtProSimpleLabel m_Description;
		private VtProAdvancedButton m_Button;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 1
			};

			m_Name = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2
			};

			m_Description = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 3
			};

			m_Button = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				DigitalEnableJoin = 2,
				AnalogModeJoin = 1
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Icon;
			yield return m_Name;
			yield return m_Description;
			yield return m_Button;
		}
	}
}
