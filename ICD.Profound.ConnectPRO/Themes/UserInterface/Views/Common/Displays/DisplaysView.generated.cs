using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public sealed partial class DisplaysView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_DisplaysList;

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
				DigitalVisibilityJoin = 112
			};

			m_DisplaysList = new VtProSubpageReferenceList(2, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 2,
				DigitalJoinIncrement = 4,
				AnalogJoinIncrement = 2,
				SerialJoinIncrement = 3
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_DisplaysList;
		}
	}
}
