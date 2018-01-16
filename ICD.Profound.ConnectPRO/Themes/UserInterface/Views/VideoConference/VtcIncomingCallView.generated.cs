using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.VideoConference
{
	public sealed partial class VtcIncomingCallView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_AnswerButton;
		private VtProButton m_IgnoreButton;

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
				DigitalVisibilityJoin = 50
			};

			m_AnswerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 513
			};

			m_IgnoreButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 517
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_AnswerButton;
			yield return m_IgnoreButton;
		}
	}
}
