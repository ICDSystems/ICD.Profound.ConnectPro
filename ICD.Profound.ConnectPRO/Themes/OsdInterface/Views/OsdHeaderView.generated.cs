using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.TextControls;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	public partial class OsdHeaderView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_RoomName;
		private VtProSimpleLabel m_TimeLabel;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent);

			m_RoomName = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 10
			};

			m_TimeLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 9
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RoomName;
			yield return m_TimeLabel;
		}
	}
}
