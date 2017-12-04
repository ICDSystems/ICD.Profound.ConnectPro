using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	public sealed partial class SourceSelectSingleView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_SourceList;

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
				DigitalVisibilityJoin = 999
			};

			m_SourceList = new VtProSubpageReferenceList(3, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 8,
				DigitalJoinIncrement = 3,
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
			yield return m_SourceList;
		}
	}
}
