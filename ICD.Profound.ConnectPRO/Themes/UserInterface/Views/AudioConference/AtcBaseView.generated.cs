using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.AudioConference
{
	public sealed partial class AtcBaseView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_CloseButton;
		private VtProFormattedText m_ActiveNumberLabel;
		private VtProFormattedText m_DialNumberLabel;
		private VtProButton m_DialButton;
		private VtProButton m_HangupButton;
		private VtProButton m_ClearButton;
		private VtProSimpleKeypad m_DialKeypad;

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
				DigitalVisibilityJoin = 150
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 113
			};

			m_ActiveNumberLabel = new VtProFormattedText(panel, m_Subpage);
			m_ActiveNumberLabel.SerialLabelJoins.Add(651);

			m_DialNumberLabel = new VtProFormattedText(panel, m_Subpage);
			m_DialNumberLabel.SerialLabelJoins.Add(650);

			m_DialButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 650,
				DigitalEnableJoin = 650
			};

			m_HangupButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 651,
				DigitalEnableJoin = 651
			};

			m_ClearButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 652,
				DigitalEnableJoin = 652
			};

			m_DialKeypad = new VtProSimpleKeypad(650, panel as IPanelDevice, m_Subpage)
			{
				MiscButtonOneChar = '*',
				MiscButtonTwoChar = '#'
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
			yield return m_ActiveNumberLabel;
			yield return m_DialNumberLabel;
			yield return m_DialButton;
			yield return m_HangupButton;
			yield return m_ClearButton;
			yield return m_DialKeypad;
		}
	}
}
