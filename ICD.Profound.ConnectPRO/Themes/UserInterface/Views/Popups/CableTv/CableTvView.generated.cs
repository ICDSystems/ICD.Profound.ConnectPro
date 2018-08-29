using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.CableTv
{
	public sealed partial class CableTvView
	{
		private VtProSubpage m_Subpage;

		private VtProButton m_CloseButton;

		private VtProButton m_GuideButton;
		private VtProButton m_ExitButton;
		private VtProButton m_PowerButton;

		private VtProDPad m_MenuDirections;
		private VtProSimpleKeypad m_NumberKeypad;

		private VtProButton m_ChannelUpButton;
		private VtProButton m_ChannelDownButton;
		private VtProButton m_PageUpButton;
		private VtProButton m_PageDownButton;

		private VtProButton m_SwipeLeftButton;
		private VtProButton m_SwipeRightButton;

		private VtProSubpageReferenceList m_ChannelList;

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
				DigitalVisibilityJoin = 213
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 113
			};

			m_GuideButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 565
			};

			m_ExitButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 566
			};

			m_PowerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 567
			};

			m_MenuDirections = new VtProDPad(12, panel as IPanelDevice, m_Subpage);

			m_NumberKeypad = new VtProSimpleKeypad(11, panel as IPanelDevice, m_Subpage);

			m_ChannelUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 550
			};

			m_ChannelDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 551
			};

			m_PageUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 552
			};

			m_PageDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 553
			};

			m_SwipeLeftButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 95
			};

			m_SwipeRightButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 95
			};

			m_ChannelList = new VtProSubpageReferenceList(14, panel as IPanelDevice, m_Subpage)
			{
				DigitalJoinIncrement = 1,
				SerialJoinIncrement = 1,
				MaxSize = 8
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_GuideButton;
			yield return m_ExitButton;
			yield return m_PowerButton;
			yield return m_Subpage;
			yield return m_CloseButton;
			yield return m_MenuDirections;
			yield return m_NumberKeypad;
			yield return m_ChannelUpButton;
			yield return m_ChannelDownButton;
			yield return m_PageUpButton;
			yield return m_PageDownButton;
			yield return m_SwipeLeftButton;
			yield return m_SwipeRightButton;
			yield return m_ChannelList;
		}
	}
}
