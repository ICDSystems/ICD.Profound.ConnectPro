using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts
{
	public sealed partial class VtcContactsView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_ContactList;
		private VtProButton m_DirectoryButton;
		private VtProButton m_FavoritesButton;
		private VtProButton m_RecentsButton;
		private VtProButton m_CallButton;
		private VtProButton m_HangupButton;
		private VtProButton m_BackButton;
		private VtProButton m_HomeButton;
		private VtProButton m_SearchButton;
		private VtProButton m_ManualDialButton;

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

			m_ContactList = new VtProSubpageReferenceList(601, panel as IPanelDevice, m_Subpage)
			{
				DigitalJoinIncrement = 3,
				AnalogJoinIncrement = 0,
				SerialJoinIncrement = 1,
				MaxSize = 20
			};

			m_DirectoryButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 119
			};

			m_FavoritesButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 132
			};

			m_RecentsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 124
			};

			m_CallButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 603,
				DigitalEnableJoin = 607
			};

			m_HangupButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 605,
				DigitalEnableJoin = 608
			};

			m_BackButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 611,
				DigitalVisibilityJoin = 614
			};

			m_HomeButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 612,
				DigitalVisibilityJoin = 615
			};

			m_SearchButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 613,
				DigitalVisibilityJoin = 616
			};

			m_ManualDialButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 604
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
			yield return m_CallButton;
			yield return m_HangupButton;
			yield return m_BackButton;
			yield return m_HomeButton;
			yield return m_SearchButton;
			yield return m_ManualDialButton;
		}
	}
}

