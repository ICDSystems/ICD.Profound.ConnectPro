using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Zoom
{
	public sealed partial class SettingsZoomAdvancedView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_AudioProcessingButton;
		private VtProButton m_AudioReverbButton;

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
				DigitalVisibilityJoin = 952
			};

			m_AudioProcessingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 411
			};

			m_AudioReverbButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 412
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_AudioProcessingButton;
			yield return m_AudioReverbButton;
		}
	}
}
