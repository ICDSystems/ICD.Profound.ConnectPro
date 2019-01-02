using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Contacts
{
	public sealed partial class VtcContactsPolycomView
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_ContactList;
		private VtProDPad m_DPad;
		private VtProButton m_NavigationButton;
		private VtProButton m_LocalButton;
		private VtProButton m_RecentsButton;
		private VtProButton m_CallButton;
		private VtProButton m_HangupButton;
		private VtProButton m_BackButton;
		private VtProButton m_HomeButton;
		private VtProButton m_DirectoryButton;
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
				DigitalVisibilityJoin = 133
			};

			m_ContactList = new VtProSubpageReferenceList(901, panel as IPanelDevice, m_Subpage)
			{
				DigitalVisibilityJoin = 931,

				DigitalJoinIncrement = 3,
				AnalogJoinIncrement = 0,
				SerialJoinIncrement = 1,
				MaxSize = 20
			};

			m_DPad = new VtProDPad(902, panel as IPanelDevice, m_Subpage)
			{
				DigitalVisibilityJoin = 930
			};

			m_NavigationButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 910
			};

			m_LocalButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 919
			};

			m_RecentsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 924
			};

			m_CallButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 903,
				DigitalEnableJoin = 907
			};

			m_HangupButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 905,
				DigitalEnableJoin = 908
			};

			m_BackButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 911,
				DigitalVisibilityJoin = 914
			};

			m_HomeButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 912,
				DigitalVisibilityJoin = 915
			};

			m_DirectoryButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 913,
				DigitalVisibilityJoin = 916
			};

			m_ManualDialButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 904
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
			yield return m_NavigationButton;
			yield return m_LocalButton;
			yield return m_RecentsButton;
			yield return m_CallButton;
			yield return m_HangupButton;
			yield return m_BackButton;
			yield return m_HomeButton;
			yield return m_DirectoryButton;
			yield return m_ManualDialButton;
		}
	}
}

