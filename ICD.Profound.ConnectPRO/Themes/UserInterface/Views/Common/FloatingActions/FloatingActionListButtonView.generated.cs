using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.FloatingActions
{
	public sealed partial class FloatingActionListButtonView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_Button;
		private VtProDynamicIconObject m_Icon;

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
				DigitalVisibilityJoin = 41
			};

			m_Button = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 43,
				DigitalEnableJoin = 44,
			};

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 1200
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
		}
	}
}
