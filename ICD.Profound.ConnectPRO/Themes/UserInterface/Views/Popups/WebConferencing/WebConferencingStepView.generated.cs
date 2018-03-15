using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.WebConferencing
{
	public sealed partial class WebConferencingStepView
	{
		private VtProSubpage m_Subpage;
		private VtProFormattedText m_StepLabel;
		private VtProFormattedText m_InstructionLabel;
		private VtProImageObject m_InstructionImage;
		private VtProButton m_BackButton;
		private VtProButton m_ForwardButton;
		private VtProButton m_CloseButton;

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
				DigitalVisibilityJoin = 107
			};

			m_StepLabel = new VtProFormattedText(panel, m_Subpage)
			{
				IndirectTextJoin = 501
			};

			m_InstructionLabel = new VtProFormattedText(panel, m_Subpage)
			{
				IndirectTextJoin = 502
			};

			m_InstructionImage = new VtProImageObject(panel, m_Subpage)
			{
				SerialGraphicsJoin = 500
			};

			m_BackButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 542,
				DigitalVisibilityJoin = 540
			};

			m_ForwardButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 543,
				DigitalVisibilityJoin = 541
			};

			m_CloseButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 113
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_StepLabel;
			yield return m_InstructionLabel;
			yield return m_InstructionImage;
			yield return m_BackButton;
			yield return m_ForwardButton;
			yield return m_CloseButton;
		}
	}
}
