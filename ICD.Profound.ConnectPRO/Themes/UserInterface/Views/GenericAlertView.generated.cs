using System;
using System.Collections.Generic;
using System.Text;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	public partial class GenericAlertView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_AlertMessageLabel;
		private VtProDynamicButtonList m_Buttons;

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
				DigitalVisibilityJoin = 7
			};

			m_AlertMessageLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				MultilineSupport = true,
				IndirectTextJoin = 6
			};

			m_Buttons = new VtProDynamicButtonList(30, panel as IPanelDevice, parent)
			{
				MaxSize = 2
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_AlertMessageLabel;
			yield return m_Buttons;
		}
	}
}
