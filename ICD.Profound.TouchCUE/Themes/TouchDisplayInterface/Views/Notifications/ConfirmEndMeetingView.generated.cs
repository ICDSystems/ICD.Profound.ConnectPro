using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Notifications
{
	public sealed partial class ConfirmEndMeetingView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_YesButton;
		private VtProButton m_CancelButton;
		private VtProSimpleLabel m_ConfirmText;

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
				DigitalVisibilityJoin = 310
			};

			m_YesButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 311
			};

			m_CancelButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 312
			};

			m_ConfirmText = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 310
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_YesButton;
			yield return m_CancelButton;
			yield return m_ConfirmText;
		}
	}
}
