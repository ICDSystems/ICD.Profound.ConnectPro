using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
    public sealed partial class OsdWelcomeView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_RoomLabel;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 100
			};

			m_RoomLabel = new VtProSimpleLabel(panel, parent)
			{
				IndirectTextJoin = 101
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RoomLabel;
		}
	}
}
