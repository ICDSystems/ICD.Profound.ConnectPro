using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	public partial class HelloView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_Label;

		public HelloView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public HelloView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index) : base(panel, theme, parent, index)
		{
		}

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 400
			};

			m_Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 401,
				DigitalEnableJoin = 401
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Label;
		}
	}
}
