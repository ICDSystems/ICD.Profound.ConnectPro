using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.ActiveCalls
{
	public sealed partial class VtcActiveCallsView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_HangupList;
		private VtProButton m_HangupAllButton;

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
				DigitalVisibilityJoin = 122
			};

			m_HangupList = new VtProSubpageReferenceList(622, panel as IPanelDevice, m_Subpage)
			{
				DigitalJoinIncrement = 3,
				SerialJoinIncrement = 1,
				MaxSize = 20
			};

			m_HangupAllButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 606
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_HangupList;
			yield return m_HangupAllButton;
		}
	}
}
