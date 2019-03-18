using System;
using System.Collections.Generic;
using System.Text;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public partial class ReferencedRouteListItemView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_RoomLabel;
		private VtProSimpleLabel m_DisplayLabel;
		private VtProSimpleLabel m_SourceLabel;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_RoomLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};

			m_DisplayLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2
			};

			m_SourceLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 3
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RoomLabel;
			yield return m_DisplayLabel;
			yield return m_SourceLabel;
		}
	}
}
