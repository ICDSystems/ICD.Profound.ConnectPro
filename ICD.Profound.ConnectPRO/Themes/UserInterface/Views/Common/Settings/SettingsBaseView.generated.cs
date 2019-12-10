using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	public sealed partial class SettingsBaseView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_ItemList;
		private VtProButton m_BackButton;
		private VtProButton m_CloseButton;
		private VtProFormattedText m_TitleLabel;

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
				DigitalVisibilityJoin = 141
			};

			m_ItemList = new VtProDynamicButtonList(652, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 20
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 160
			};

			m_BackButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 158,
				DigitalVisibilityJoin = 159
			};

			m_TitleLabel = new VtProFormattedText(panel, m_Subpage)
			{
				SerialLabelJoins = { 403, 407}
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ItemList;
			yield return m_CloseButton;
			yield return m_BackButton;
			yield return m_TitleLabel;
		}
	}
}
