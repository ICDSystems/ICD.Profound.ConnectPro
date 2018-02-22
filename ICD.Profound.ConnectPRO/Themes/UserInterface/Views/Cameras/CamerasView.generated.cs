using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Cameras
{
	public sealed partial class CamerasView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_ButtonList;
		private VtProButton m_CloseButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 96
			};

			m_ButtonList = new VtProDynamicButtonList(4, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 8
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 526
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ButtonList;
			yield return m_CloseButton;
		}
	}
}
