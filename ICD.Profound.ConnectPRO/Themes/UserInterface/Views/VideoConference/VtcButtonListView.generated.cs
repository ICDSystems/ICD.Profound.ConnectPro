using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcButtonListView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicButtonList m_ButtonList;

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
				DigitalVisibilityJoin = 129
			};

			m_ButtonList = new VtProDynamicButtonList(629, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 3
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
		}
	}
}
