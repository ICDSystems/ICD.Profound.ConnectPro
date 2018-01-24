using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public sealed partial class ReferencedDisplaysView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_BackgroundButton;
		private VtProDynamicIconObject m_Icon;
		private VtProSimpleLabel m_Line1Label;
		private VtProSimpleLabel m_Line2Label;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_BackgroundButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				AnalogModeJoin = 1
			};

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 2
			};

			m_Line1Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};

			m_Line2Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_BackgroundButton;
			yield return m_Icon;
			yield return m_Line1Label;
			yield return m_Line2Label;
		}
	}
}
