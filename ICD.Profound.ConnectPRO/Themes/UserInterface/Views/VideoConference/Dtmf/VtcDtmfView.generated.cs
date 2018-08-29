using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Keypads;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference.Dtmf
{
	public sealed partial class VtcDtmfView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleKeypad m_Keypad;
		private VtProSubpageReferenceList m_ConferenceSourceList;

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
				DigitalVisibilityJoin = 123
			};

			m_Keypad = new VtProSimpleKeypad(623, panel as IPanelDevice, m_Subpage)
			{
				MiscButtonOneChar = '*',
				MiscButtonTwoChar = '#'
			};

			m_ConferenceSourceList = new VtProSubpageReferenceList(622, panel as IPanelDevice, m_Subpage)
			{
				DigitalJoinIncrement = 3,
				AnalogJoinIncrement = 0,
				SerialJoinIncrement = 1,
				MaxSize = 20
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Keypad;
			yield return m_ConferenceSourceList;
		}
	}
}
