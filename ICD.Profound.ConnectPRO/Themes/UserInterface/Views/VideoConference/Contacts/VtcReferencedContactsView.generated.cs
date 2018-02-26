using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts
{
	public sealed partial class VtcReferencedContactsView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_BackgroundButton;
		private VtProSimpleLabel m_ContactNameLabel;
		private VtProButton m_FavoriteButton;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_BackgroundButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};

			m_ContactNameLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};

			m_FavoriteButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2,
				DigitalVisibilityJoin = 3
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
			yield return m_ContactNameLabel;
			yield return m_FavoriteButton;
		}
	}
}
