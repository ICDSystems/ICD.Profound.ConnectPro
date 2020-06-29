using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class TouchFreeCancelPromptView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_Message;
		private VtProButton m_CancelMeetingStartButton;

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
				DigitalVisibilityJoin = 37
			};

			m_Message = new VtProSimpleLabel(panel, m_Subpage);

			m_Message.SerialLabelJoins.Add(11);
			

			m_CancelMeetingStartButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 609
			};
			
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Message;
			yield return m_CancelMeetingStartButton;
		}
		
	}
}