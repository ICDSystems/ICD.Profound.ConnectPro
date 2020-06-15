using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Administrative
{
	public sealed partial class SettingsPowerView
	{
		private VtProSubpage m_Subpage;

		private VtProTabButton m_DaysButtons;

		private VtProButton m_WakeHourIncrementButton;
		private VtProButton m_WakeHourDecrementButton;
		private VtProButton m_WakeMinuteIncrementButton;
		private VtProButton m_WakeMinuteDecrementButton;

		private VtProButton m_SleepHourIncrementButton;
		private VtProButton m_SleepHourDecrementButton;
		private VtProButton m_SleepMinuteIncrementButton;
		private VtProButton m_SleepMinuteDecrementButton;

		private VtProSimpleLabel m_WakeHourLabel;
		private VtProSimpleLabel m_WakeMinuteLabel;

		private VtProSimpleLabel m_SleepHourLabel;
		private VtProSimpleLabel m_SleepMinuteLabel;

		private VtProButton m_DisplayPowerToggleButton;
		private VtProButton m_EnableWakeToggleButton;
		private VtProButton m_EnableSleepToggleButton;

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

			m_DaysButtons = new VtProTabButton(402, panel as IPanelDevice, m_Subpage);

			m_WakeHourIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 802,
				DigitalEnableJoin = 398
			};

			m_WakeHourDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 803,
				DigitalEnableJoin = 398
			};

			m_WakeMinuteIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 804,
				DigitalEnableJoin = 398
			};

			m_WakeMinuteDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 805,
				DigitalEnableJoin = 398
			};

			m_WakeHourLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 810,
				DigitalEnableJoin = 398
			};

			m_WakeMinuteLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 811,
				DigitalEnableJoin = 398
			};

			m_SleepHourIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 806,
				DigitalEnableJoin = 399
			};

			m_SleepHourDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 807,
				DigitalEnableJoin = 399
			};

			m_SleepMinuteIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 808,
				DigitalEnableJoin = 399
			};

			m_SleepMinuteDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 809,
				DigitalEnableJoin = 399
			};

			m_SleepHourLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 812,
				DigitalEnableJoin = 399
			};

			m_SleepMinuteLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 813,
				DigitalEnableJoin = 399
			};

			m_DisplayPowerToggleButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 408
			};

			m_EnableWakeToggleButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 406
			};

			m_EnableSleepToggleButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 407
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;

			yield return m_DaysButtons;

			yield return m_WakeHourIncrementButton;
			yield return m_WakeHourDecrementButton;
			yield return m_WakeMinuteIncrementButton;
			yield return m_WakeMinuteDecrementButton;

			yield return m_SleepHourIncrementButton;
			yield return m_SleepHourDecrementButton;
			yield return m_SleepMinuteIncrementButton;
			yield return m_SleepMinuteDecrementButton;

			yield return m_WakeHourLabel;
			yield return m_WakeMinuteLabel;

			yield return m_SleepHourLabel;
			yield return m_SleepMinuteLabel;

			yield return m_DisplayPowerToggleButton;
			yield return m_EnableWakeToggleButton;
			yield return m_EnableSleepToggleButton;
		}
	}
}
