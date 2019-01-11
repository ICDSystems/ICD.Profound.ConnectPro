using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public partial class WtcShareView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_SwipeLabels;
		private VtProDynamicButtonList m_SourceList;
		private VtProButton m_ShareButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 742
			};

			m_SwipeLabels = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 5
			};

			m_SourceList = new VtProDynamicButtonList(707, panel as IPanelDevice, m_Subpage);

			m_ShareButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 731,
				DigitalEnableJoin = 732
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_SwipeLabels;
			yield return m_SourceList;
			yield return m_ShareButton;
		}
	}
}