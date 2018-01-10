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
		private VtProGuage m_Guage;
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

			m_Guage = new VtProGuage(panel, m_Subpage)
			{
				// TODO AnalogFeedbackJoin = 0,
				// TODO DigitalEnableJoin = 0
			};

			m_VolumeUpButton = new VtProButton(panel, m_Subpage)
			{
				// TODO DigitalPressJoin = 0
			};

			m_VolumeDownButton = new VtProButton(panel, m_Subpage)
			{
				// TODO DigitalPressJoin = 0
			};

			m_MuteButton = new VtProButton(panel, m_Subpage)
			{
				// TODO DigitalPressJoin = 0
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Guage;
			yield return m_VolumeUpButton;
			yield return m_VolumeDownButton;
			yield return m_MuteButton;
		}
	}
}
