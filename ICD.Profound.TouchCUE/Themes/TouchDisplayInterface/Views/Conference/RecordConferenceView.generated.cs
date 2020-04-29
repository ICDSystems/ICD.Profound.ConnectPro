using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference
{
	public partial class RecordConferenceView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_RecordButton;
		private VtProButton m_StopButton;
		private VtProSimpleLabel m_RecordingAnimation;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 940
			};

			m_RecordButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 941,
				DigitalEnableJoin = 942
			};

			m_StopButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 943,
				DigitalEnableJoin = 944
			};

			// just for the digital join
			m_RecordingAnimation = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalEnableJoin = 945
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RecordButton;
			yield return m_StopButton;
			yield return m_RecordingAnimation;
		}
	}
}
