using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Sources
{
	public sealed partial class ReferencedSourceSelectView
	{
		private VtProSubpage m_Subpage;
		private VtProAdvancedButton m_BackgroundButton;
		private VtProDynamicIconObject m_Icon;
		private VtProSimpleLabel m_Line1Label;
		private VtProSimpleLabel m_Line2Label;
		private VtProSimpleLabel m_FeedbackLabel;
		private VtProAdvancedButton m_RoutedButton;

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

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 4
			};

			m_Line1Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};

			m_Line2Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2
			};

			m_FeedbackLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 3
			};

			m_RoutedButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 2,
				AnalogModeJoin = 2
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
			yield return m_Icon;
			yield return m_Line1Label;
			yield return m_Line2Label;
			yield return m_FeedbackLabel;
		}
	}
}
