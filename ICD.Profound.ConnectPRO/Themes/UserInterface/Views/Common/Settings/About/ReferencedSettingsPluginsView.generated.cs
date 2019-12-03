using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.About
{
	public sealed partial class ReferencedSettingsPluginsView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_PluginLabel;
		private VtProSimpleLabel m_VersionLabel;
		private VtProSimpleLabel m_DateLabel;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_PluginLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};

			m_VersionLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2
			};

			m_DateLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 3
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_PluginLabel;
			yield return m_VersionLabel;
			yield return m_DateLabel;
		}
	}
}
