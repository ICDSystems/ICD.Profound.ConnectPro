using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	public sealed partial class SettingsSystemPowerView
	{
		private VtProSubpage m_Subpage;

		private VtProButton m_WeekdaysButton;
		private VtProButton m_WeekendsButton;

		private VtProButton m_WakeHourIncrementButton;
		private VtProButton m_WakeHourDecrementButton;
		private VtProButton m_WakeMinuteIncrementButton;
		private VtProButton m_WakeMinuteDecrementButton;

		private VtProButton m_SleepHourIncrementButton;
		private VtProButton m_SleepHourDecrementButton;
		private VtProButton m_SleepMinuteIncrementButton;
		private VtProButton m_SleepMinuteDecrementButton;

		private VtProButton m_EnableButton;
		private VtProButton m_DisableButton;

		private VtProButton m_WakeButton;
		private VtProButton m_SleepButton;

		private VtProSimpleLabel m_WakeHourLabel;
		private VtProSimpleLabel m_WakeMinuteLabel;

		private VtProSimpleLabel m_SleepHourLabel;
		private VtProSimpleLabel m_SleepMinuteLabel;

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
				DigitalVisibilityJoin = 142
			};

			m_WeekdaysButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 800
			};

			m_WeekendsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 801
			};

			m_WakeHourIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 802
			};

			m_WakeHourDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 803
			};

			m_WakeMinuteIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 804
			};

			m_WakeMinuteDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 805
			};

			m_SleepHourIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 806
			};

			m_SleepHourDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 807
			};

			m_SleepMinuteIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 808
			};

			m_SleepMinuteDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 809
			};

			m_EnableButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 810
			};

			m_DisableButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 811
			};

			m_WakeButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 812
			};

			m_SleepButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 813
			};

			m_WakeHourLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 810
			};

			m_WakeMinuteLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 811
			};

			m_SleepHourLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 812
			};

			m_SleepMinuteLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 813
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;

			yield return m_WeekdaysButton;
			yield return m_WeekendsButton;

			yield return m_WakeHourIncrementButton;
			yield return m_WakeHourDecrementButton;
			yield return m_WakeMinuteIncrementButton;
			yield return m_WakeMinuteDecrementButton;

			yield return m_SleepHourIncrementButton;
			yield return m_SleepHourDecrementButton;
			yield return m_SleepMinuteIncrementButton;
			yield return m_SleepMinuteDecrementButton;

			yield return m_EnableButton;
			yield return m_DisableButton;

			yield return m_WakeButton;
			yield return m_SleepButton;

			yield return m_WakeHourLabel;
			yield return m_WakeMinuteLabel;

			yield return m_SleepHourLabel;
			yield return m_SleepMinuteLabel;
		}
	}
}
