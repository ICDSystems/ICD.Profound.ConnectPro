using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public partial class WtcRecordingView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_StartRecordingButton;
		private VtProButton m_StopRecordingButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 743
			};

			m_StartRecordingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 727,
				DigitalEnableJoin = 728
			};
			m_StopRecordingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 729,
				DigitalEnableJoin = 730
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_StartRecordingButton;
			yield return m_StopRecordingButton;
		}
	}
}