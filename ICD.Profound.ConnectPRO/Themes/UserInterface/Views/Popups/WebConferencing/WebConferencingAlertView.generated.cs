using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.WebConferencing
{
	public sealed partial class WebConferencingAlertView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_DismissButton;
		private VtProSubpageReferenceList m_AppList;

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
				DigitalVisibilityJoin = 106
			};

			m_DismissButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 140
			};

			m_AppList = new VtProSubpageReferenceList(15, panel as IPanelDevice, parent)
			{
				DigitalJoinIncrement = 1,
				SerialJoinIncrement = 2,
				MaxSize = 8
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_DismissButton;
			yield return m_AppList;
		}
	}
}
