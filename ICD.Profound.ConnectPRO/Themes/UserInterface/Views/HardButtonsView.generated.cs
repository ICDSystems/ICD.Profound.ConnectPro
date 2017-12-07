using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	public sealed partial class HardButtonsView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_PowerButton;
		private VtProButton m_HomeButton;
		private VtProButton m_LightButton;
		private VtProButton m_VolumeUpButton;
		private VtProButton m_VolumeDownButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_PowerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};

			m_HomeButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2
			};

			m_LightButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 3
			};

			m_VolumeUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 4
			};

			m_VolumeDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 5
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_PowerButton;
			yield return m_HomeButton;
			yield return m_LightButton;
			yield return m_VolumeUpButton;
			yield return m_VolumeDownButton;
		}
	}
}
