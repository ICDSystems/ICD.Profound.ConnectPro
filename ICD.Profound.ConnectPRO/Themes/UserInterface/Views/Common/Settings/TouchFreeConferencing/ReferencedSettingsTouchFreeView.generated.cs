using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.TouchFreeConferencing
{
	public sealed partial class ReferencedSettingsTouchFreeView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicIconObject m_Icon;
		private VtProSimpleLabel m_Text;
		private VtProButton m_Device;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				DynamicIconSerialJoin = 2
			};

			m_Text = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};

			m_Device = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Icon;
			yield return m_Text;
			yield return m_Device;
		}
	}
}
