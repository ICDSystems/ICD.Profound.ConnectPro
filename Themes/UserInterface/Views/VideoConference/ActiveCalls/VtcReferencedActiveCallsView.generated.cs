using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.ActiveCalls
{
	public sealed partial class VtcReferencedActiveCallsView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_HangupButton;
		private VtProSimpleLabel m_Label;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_HangupButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2,
				DigitalVisibilityJoin = 3
			};

			m_Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_HangupButton;
			yield return m_Label;
		}
	}
}
