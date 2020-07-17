using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Headers
{
	public partial class OsdHeaderView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_RoomName;
		private VtProSimpleLabel m_TimeLabel;
		private VtProImageObject m_TouchFreeFace;

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

			m_TouchFreeFace = new VtProImageObject(panel, m_Subpage)
			{
				ModeAnalogJoin = 800
			};

		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RoomName;
			yield return m_TimeLabel;
			yield return m_TouchFreeFace;
		}
	}
}
