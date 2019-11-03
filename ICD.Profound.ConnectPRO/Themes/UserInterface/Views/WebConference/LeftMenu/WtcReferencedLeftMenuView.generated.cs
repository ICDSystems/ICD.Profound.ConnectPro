using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.LeftMenu
{
	public sealed partial class WtcReferencedLeftMenuView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_Button;
		private VtProDynamicIconObject m_Icon;
		private VtProAdvancedButton m_StatusButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				IndirectTextJoin = 1
			};

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 2
			};

			m_StatusButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				AnalogModeJoin = 1,
				DigitalVisibilityJoin = 2
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Button;
			yield return m_Icon;
			yield return m_StatusButton;
		}
	}
}
