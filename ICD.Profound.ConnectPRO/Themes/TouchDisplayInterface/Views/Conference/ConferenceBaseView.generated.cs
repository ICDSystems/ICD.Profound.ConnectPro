using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference
{
	public partial class ConferenceBaseView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_CloseButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				// TODO - joins
				// visibliity join just to make it not think its always visible
				DigitalVisibilityJoin = 800
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				// TODO - joins
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CloseButton;
		}
	}
}