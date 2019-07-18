using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public partial class ReferencedDisplayView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_BackgroundButton;
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
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_BackgroundButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				AnalogModeJoin = 1
			};

			m_SpeakerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 2,
				DigitalPressJoin = 3
			};

			m_DisplayIcon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 3
			};

			m_SourceLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 4
			};

			m_Line1Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};

			m_Line2Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2
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
			yield return m_SpeakerButton;
			yield return m_DisplayIcon;
			yield return m_SourceLabel;
			yield return m_Line1Label;
			yield return m_Line2Label;
		}
	}
}
