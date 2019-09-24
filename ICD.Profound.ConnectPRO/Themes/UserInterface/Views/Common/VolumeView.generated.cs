using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Guages;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class VolumeView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_CloseButton;
		private VtProGauge m_Gauge;
		private VtProButton m_VolumeUpButton;
		private VtProButton m_VolumeDownButton;
		private VtProButton m_MuteButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 11
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 46
			};

			m_Gauge = new VtProGauge(panel, m_Subpage)
			{
				AnalogFeedbackJoin = 500,
				DigitalEnableJoin = 525
			};

			m_VolumeUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 523,
				DigitalEnableJoin = 526
			};

			m_VolumeDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 522,
				DigitalEnableJoin = 526
			};

			m_MuteButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 524,
				DigitalEnableJoin = 526
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CloseButton;
			yield return m_Gauge;
			yield return m_VolumeUpButton;
			yield return m_VolumeDownButton;
			yield return m_MuteButton;
		}
	}
}
