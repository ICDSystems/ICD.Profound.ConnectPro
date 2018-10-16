using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Popups
{
	public sealed partial class OsdMuteView
	{
		private VtProSubpage m_Subpage;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 500
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
		}
	}
}
