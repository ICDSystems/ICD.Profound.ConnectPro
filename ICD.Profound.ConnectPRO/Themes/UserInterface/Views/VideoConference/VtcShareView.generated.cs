using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcShareView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_SwipeLabels;
		private VtProDynamicButtonList m_ButtonList;
		private VtProButton m_ShareButton;

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
				DigitalVisibilityJoin = 121
			};

			m_SwipeLabels = new VtProSimpleLabel(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 5
			};

			m_ButtonList = new VtProDynamicButtonList(621, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 8
			};

			m_ShareButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 641,
				DigitalEnableJoin = 642
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_SwipeLabels;
			yield return m_ButtonList;
			yield return m_ShareButton;
		}
	}
}
