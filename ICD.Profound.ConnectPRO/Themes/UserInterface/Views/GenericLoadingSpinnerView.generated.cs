using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	public partial class GenericLoadingSpinnerView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_Label;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 10
			};

			m_Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 7
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Label;
		}
	}
}