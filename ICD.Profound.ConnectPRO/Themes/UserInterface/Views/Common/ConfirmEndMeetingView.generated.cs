using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class ConfirmEndMeetingView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_YesButton;
		private VtProButton m_CancelButton;
		private VtProButton m_ShutdownButton;

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
				DigitalVisibilityJoin = 30
			};

			m_YesButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 31
			};

			m_CancelButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 32
			};

			m_ShutdownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
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
			yield return m_ShutdownButton;
		}
	}
}
