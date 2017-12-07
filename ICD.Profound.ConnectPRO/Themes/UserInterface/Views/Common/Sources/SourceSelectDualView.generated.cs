using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	public sealed partial class SourceSelectDualView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_SourceList;
		private VtProButton m_LeftArrowButton;
		private VtProButton m_RightArrowButton;

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
				DigitalVisibilityJoin = 110
			};

			m_SourceList = new VtProSubpageReferenceList(1, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 20,
				DigitalJoinIncrement = 3,
				AnalogJoinIncrement = 2,
				SerialJoinIncrement = 3
			};

			m_LeftArrowButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 1
			};

			m_RightArrowButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 1
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
			yield return m_LeftArrowButton;
			yield return m_RightArrowButton;
		}
	}
}
