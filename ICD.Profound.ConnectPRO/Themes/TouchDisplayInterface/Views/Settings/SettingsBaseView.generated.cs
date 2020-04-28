using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings
{
	public sealed partial class SettingsBaseView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_PrimaryItemList;
		private VtProDynamicButtonList m_SecondaryItemList;
		private VtProButton m_CloseButton;

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
				DigitalVisibilityJoin = 1000
			};

			m_PrimaryItemList = new VtProDynamicButtonList(1000, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 50
			};

			m_SecondaryItemList = new VtProDynamicButtonList(1001, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 50,
				DigitalVisibilityJoin = 1001
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1002
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_PrimaryItemList;
			yield return m_SecondaryItemList;
			yield return m_CloseButton;
		}
	}
}
