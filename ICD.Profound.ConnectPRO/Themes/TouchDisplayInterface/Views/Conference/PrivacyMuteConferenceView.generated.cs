using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference
{
	public partial class PrivacyMuteConferenceView
	{
		private VtProSubpage m_Subpage;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 895
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
		}
	}
}
