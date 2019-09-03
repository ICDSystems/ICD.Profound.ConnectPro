using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	public sealed partial class SettingsPromptView
	{
		private VtProSubpage m_Subpage;
		private VtProImageObject m_Image;
		private VtProSimpleLabel m_HelpLabel;

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
				DigitalVisibilityJoin = 147
			};

			m_Image = new VtProImageObject(panel, m_Subpage)
			{
				SerialGraphicsJoin = 402
			};

			m_HelpLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 401
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Image;
			yield return m_HelpLabel;
		}
	}
}
