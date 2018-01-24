using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcContactsView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_ContactList;
		private VtProButton m_DirectoryButton;
		private VtProButton m_FavoritesButton;
		private VtProButton m_RecentsButton;
		private VtProTextEntry m_SearchBar;
		private VtProButton m_CallButton;
		private VtProButton m_HangupButton;

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
				DigitalVisibilityJoin = 120
			};

			m_ContactList = new VtProSubpageReferenceList(501, panel as IPanelDevice, m_Subpage)
			{
				DigitalJoinIncrement = 2,
				AnalogJoinIncrement = 0,
				SerialJoinIncrement = 1,
				MaxSize = 20
			};

			m_DirectoryButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 514
			};

			m_FavoritesButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 515
			};

			m_RecentsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 516
			};

			m_SearchBar = new VtProTextEntry(panel, m_Subpage)
			{
				IndirectTextJoin = 500,
				SerialOutputJoin = 501
			};

			m_CallButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 503,
				DigitalEnableJoin = 509
			};

			m_HangupButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 510,
				DigitalEnableJoin = 511
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ContactList;
			yield return m_DirectoryButton;
			yield return m_FavoritesButton;
			yield return m_RecentsButton;
			yield return m_SearchBar;
			yield return m_CallButton;
			yield return m_HangupButton;
	}
	}
}

