﻿using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.AudioConference
{
	public sealed partial class AtcIncomingCallView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_CloseButton;
		private VtProButton m_AnswerButton;
		private VtProButton m_RejectButton;
		private VtProFormattedText m_CallerLabel;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, m_Subpage, index)
			{
				DigitalVisibilityJoin = 151
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 602
			};

			m_AnswerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 653
			};

			m_RejectButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 654
			};

			m_CallerLabel = new VtProFormattedText(panel, m_Subpage);
			m_CallerLabel.SerialLabelJoins.Add(652);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CloseButton;
			yield return m_AnswerButton;
			yield return m_RejectButton;
			yield return m_CallerLabel;
		}
	}
}