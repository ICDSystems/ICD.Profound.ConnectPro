using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Gauges;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views
{
	public partial class VolumeView
	{
		private VtProSubpage m_Subpage;
		private VtProGauge m_VolumeGauge;
		private VtProButton m_VolumeMuteButton;
		private VtProButton m_VolumeDownButton;
		private VtProButton m_VolumeUpButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 75
			};

			m_VolumeGauge = new VtProGauge(panel, m_Subpage)
			{
				AnalogFeedbackJoin = 75
			};

			m_VolumeMuteButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 74,
				DigitalPressJoin = 76
			};

			m_VolumeDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 77
			};
			m_VolumeUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 78
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_VolumeGauge;
			yield return m_VolumeMuteButton;
			yield return m_VolumeDownButton;
			yield return m_VolumeUpButton;
		}
	}
}
