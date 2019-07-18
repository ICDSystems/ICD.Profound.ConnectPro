using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	public sealed partial class Menu2DisplaysView
	{
		private VtProSubpage m_Subpage;

		private VtProAdvancedButton m_Display1BackgroundButton;
		private VtProButton m_Display1SpeakerButton;
		private VtProDynamicIconObject m_Display1Icon;
		private VtProSimpleLabel m_Display1SourceLabel;
		private VtProSimpleLabel m_Display1Line1Label;
		private VtProSimpleLabel m_Display1Line2Label;

		private VtProAdvancedButton m_Display2BackgroundButton;
		private VtProButton m_Display2SpeakerButton;
		private VtProDynamicIconObject m_Display2Icon;
		private VtProSimpleLabel m_Display2SourceLabel;
		private VtProSimpleLabel m_Display2Line1Label;
		private VtProSimpleLabel m_Display2Line2Label;

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
				DigitalVisibilityJoin = 114
			};

			m_Display1BackgroundButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 301,
				AnalogModeJoin = 300
			};

			m_Display1Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 303
			};

			m_Display1SpeakerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 302,
				DigitalPressJoin = 303
			};

			m_Display1SourceLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 304
			};

			m_Display1Line1Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 301
			};

			m_Display1Line2Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 302
			};

			m_Display2BackgroundButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				DigitalPressJoin = 304,
				AnalogModeJoin = 301
			};

			m_Display2Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 307
			};

			m_Display2SpeakerButton = new VtProButton(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 305,
				DigitalPressJoin = 306
			};

			m_Display2SourceLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 308
			};

			m_Display2Line1Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 305
			};

			m_Display2Line2Label = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 306
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;

			yield return m_Display1BackgroundButton;
			yield return m_Display1SpeakerButton;
			yield return m_Display1Icon;
			yield return m_Display1SourceLabel;
			yield return m_Display1Line1Label;
			yield return m_Display1Line2Label;

			yield return m_Display2BackgroundButton;
			yield return m_Display2SpeakerButton;
			yield return m_Display2Icon;
			yield return m_Display2SourceLabel;
			yield return m_Display2Line1Label;
			yield return m_Display2Line2Label;
		}
	}
}
