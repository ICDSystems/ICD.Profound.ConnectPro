using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.Buttons;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	public sealed partial class DisabledAlertView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_dismissButton;

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
				DigitalVisibilityJoin = 128
			};
			m_dismissButton = new VtProButton(panel, parent)
			{
				DigitalPressJoin = 100
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_dismissButton;
		}
	}
}
