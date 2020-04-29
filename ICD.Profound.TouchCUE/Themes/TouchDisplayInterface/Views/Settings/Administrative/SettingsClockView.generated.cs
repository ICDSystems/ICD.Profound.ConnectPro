using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Settings.Administrative
{
	public sealed partial class SettingsClockView
	{
		private VtProSubpage m_Subpage;
		private VtProImageObject m_BackgroundImage;
		private VtProButton m_DayIncrementButton;
		private VtProSimpleLabel m_DayLabel;
		private VtProButton m_DayDecrementButton;
		private VtProButton m_MonthIncrementButton;
		private VtProSimpleLabel m_MonthLabel;
		private VtProButton m_MonthDecrementButton;
		private VtProButton m_YearIncrementButton;
		private VtProSimpleLabel m_YearLabel;
		private VtProButton m_YearDecrementButton;
		private VtProButton m_24HourButton;
		private VtProButton m_HourIncrementButton;
		private VtProSimpleLabel m_HourLabel;
		private VtProButton m_HourDecrementButton;
		private VtProButton m_MinuteIncrementButton;
		private VtProSimpleLabel m_MinuteLabel;
		private VtProButton m_MinuteDecrementButton;
		private VtProButton m_AmPmButton;

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
				DigitalVisibilityJoin = 1020
			};

			m_BackgroundImage = new VtProImageObject(panel, m_Subpage)
			{
				ModeAnalogJoin = 1020
			};

			m_DayIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1021
			};
			
			m_DayLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1020
			};

			m_DayDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1022
			};

			m_MonthIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1023
			};
			
			m_MonthLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1021
			};

			m_MonthDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1024
			};

			m_YearIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1025
			};

			m_YearLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1022
			};

			m_YearDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1026
			};

			m_24HourButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1027
			};

			m_HourIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1028
			};

			m_HourLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1023
			};

			m_HourDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1029
			};

			m_MinuteIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1030
			};

			m_MinuteLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1024
			};

			m_MinuteDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1031
			};

			m_AmPmButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1032,
				DigitalVisibilityJoin = 1033
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_BackgroundImage;
			yield return m_DayIncrementButton;
			yield return m_DayLabel;
			yield return m_DayDecrementButton;
			yield return m_MonthIncrementButton;
			yield return m_MonthLabel;
			yield return m_MonthDecrementButton;
			yield return m_YearIncrementButton;
			yield return m_YearLabel;
			yield return m_YearDecrementButton;
			yield return m_24HourButton;
			yield return m_HourIncrementButton;
			yield return m_HourLabel;
			yield return m_HourDecrementButton;
			yield return m_MinuteIncrementButton;
			yield return m_MinuteLabel;
			yield return m_MinuteDecrementButton;
			yield return m_AmPmButton;
		}
	}
}
