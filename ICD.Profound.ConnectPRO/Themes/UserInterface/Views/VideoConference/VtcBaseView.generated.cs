﻿using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcBaseView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_CloseButton;
		private VtProTabButton m_NavButtons;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel)
			{
				DigitalVisibilityJoin = 92
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 113
			};

			m_NavButtons = new VtProTabButton(500, panel as IPanelDevice, m_Subpage);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_CloseButton;
			yield return m_NavButtons;
		}
	}
}
