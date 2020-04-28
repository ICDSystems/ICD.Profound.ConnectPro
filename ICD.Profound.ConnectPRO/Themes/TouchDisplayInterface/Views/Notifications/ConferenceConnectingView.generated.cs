using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Notifications
{
	public partial class ConferenceConnectingView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_NotificationText;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 1200
			};

			m_NotificationText = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1200
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_NotificationText;
		}
	}
}
