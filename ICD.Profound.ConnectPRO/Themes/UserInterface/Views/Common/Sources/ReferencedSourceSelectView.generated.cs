using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	public sealed partial class ReferencedSourceSelectView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_BackgroundButton;

		private VtProMultiModeButton m_GreyIconButton;
		private VtProSimpleLabel m_GreyLine1Label;
		private VtProSimpleLabel m_GreyLine2Label;
		private VtProSimpleLabel m_GreyFeedbackLabel;

		private VtProMultiModeButton m_GreenIconButton;
		private VtProSimpleLabel m_GreenLine1Label;
		private VtProSimpleLabel m_GreenLine2Label;
		private VtProSimpleLabel m_GreenFeedbackLabel;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_BackgroundButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				AnalogModeJoin = 1
			};

			// Grey controls
			m_GreyIconButton = new VtProMultiModeButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 2,
				AnalogModeJoin = 2
			};

			m_GreyLine1Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1,
				DigitalVisibilityJoin = 2
			};

			m_GreyLine2Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2,
				DigitalVisibilityJoin = 2
			};

			m_GreyFeedbackLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 3,
				DigitalVisibilityJoin = 2
			};

			// Green controls
			m_GreenIconButton = new VtProMultiModeButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 3,
				AnalogModeJoin = 2
			};

			m_GreenLine1Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1,
				DigitalVisibilityJoin = 3
			};

			m_GreenLine2Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2,
				DigitalVisibilityJoin = 3
			};

			m_GreenFeedbackLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 3,
				DigitalVisibilityJoin = 3
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_BackgroundButton;

			yield return m_GreyIconButton;
			yield return m_GreyLine1Label;
			yield return m_GreyLine2Label;
			yield return m_GreyFeedbackLabel;

			yield return m_GreenIconButton;
			yield return m_GreenLine1Label;
			yield return m_GreenLine2Label;
			yield return m_GreenFeedbackLabel;
		}
	}
}
