using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings.Conferencing
{
	public sealed partial class SettingsDirectoryView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_HelpLabel;
		private VtProButton m_ClearCacheButton;

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
				DigitalVisibilityJoin = 1060
			};

			m_HelpLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1060
			};

			m_ClearCacheButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1061
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_HelpLabel;
			yield return m_ClearCacheButton;
		}
	}
}
