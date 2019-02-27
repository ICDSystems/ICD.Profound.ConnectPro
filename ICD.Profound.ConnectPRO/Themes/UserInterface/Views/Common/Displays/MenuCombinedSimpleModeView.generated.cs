using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public partial class MenuCombinedSimpleModeView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_AdvancedModeButton;
		private VtProButton m_DisplayButton;
		private VtProButton m_SpeakerButton;
		private VtProDynamicIconObject m_DisplayIcon;
		private VtProSimpleLabel m_SourceLabel;
		private VtProSimpleLabel m_Line1Label;
		private VtProSimpleLabel m_Line2Label;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent)
			{
				DigitalVisibilityJoin = 110
			};

			m_AdvancedModeButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 694,
				DigitalEnableJoin = 695
			};

			m_DisplayButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 311
			};

			m_SpeakerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 312,
				DigitalPressJoin = 313
			};

			m_DisplayIcon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 313
			};

			m_SourceLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 314
			};

			m_Line1Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 311
			};

			m_Line2Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 312
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_AdvancedModeButton;
			yield return m_DisplayButton;
			yield return m_SpeakerButton;
			yield return m_DisplayIcon;
			yield return m_SourceLabel;
			yield return m_Line1Label;
			yield return m_Line2Label;
		}
	}
}
