using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups
{
	public sealed partial class CableTvView
	{
		private VtProSubpage m_Subpage;

		private VtProButton m_CloseButton;

		private VtProTabButton m_MenuButtons;
		private VtProDPad m_MenuDirections;
		private VtProSimpleKeypad m_NumberKeypad;

		private VtProButton m_ChannelUpButton;
		private VtProButton m_ChannelDownButton;
		private VtProButton m_PageUpButton;
		private VtProButton m_PageDownButton;

		private VtProButton m_RedButton;
		private VtProButton m_YellowButton;
		private VtProButton m_GreenButton;
		private VtProButton m_BlueButton;

		private VtProButton m_RepeatButton;
		private VtProButton m_RewindButton;
		private VtProButton m_StopButton;
		private VtProButton m_PlayButton;
		private VtProButton m_PauseButton;
		private VtProButton m_FastForwardButton;
		private VtProButton m_RecordButton;

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
				DigitalPressJoin = 110
			};

			// TODO
			m_MenuButtons = new VtProTabButton(0, panel as IPanelDevice, m_Subpage);
			
			// TODO
			m_MenuDirections = new VtProDPad(0, panel as IPanelDevice, m_Subpage);

			// TODO
			m_NumberKeypad = new VtProSimpleKeypad(0, panel as IPanelDevice, m_Subpage);

			m_ChannelUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_ChannelDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_PageUpButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_PageDownButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_RedButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_YellowButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_GreenButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_BlueButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_RepeatButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_RewindButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_StopButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_PlayButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_PauseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_FastForwardButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};

			m_RecordButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 0 // TODO
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;

			yield return m_CloseButton;

			yield return m_MenuButtons;
			yield return m_MenuDirections;
			yield return m_NumberKeypad;

			yield return m_ChannelUpButton;
			yield return m_ChannelDownButton;
			yield return m_PageUpButton;
			yield return m_PageDownButton;

			yield return m_RedButton;
			yield return m_YellowButton;
			yield return m_GreenButton;
			yield return m_BlueButton;

			yield return m_RepeatButton;
			yield return m_RewindButton;
			yield return m_StopButton;
			yield return m_PlayButton;
			yield return m_PauseButton;
			yield return m_FastForwardButton;
			yield return m_RecordButton;
	}
	}
}
