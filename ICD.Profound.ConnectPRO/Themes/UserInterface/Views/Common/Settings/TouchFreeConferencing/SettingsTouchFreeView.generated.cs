using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.TouchFreeConferencing
{
	public sealed partial class SettingsTouchFreeView
	{
		private VtProSubpage m_Subpage;

		private VtProButton m_CountDownTimerIncrementButton;
		private VtProButton m_CountDownTimerDecrementButton;

		private VtProSubpageReferenceList m_DefaultDeviceList;

		private VtProSimpleLabel m_CountDownSecondsLabel;

		private VtProButton m_EnableZeroTouchToggleButton;

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
				DigitalVisibilityJoin = 956
			};

			m_CountDownTimerIncrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 810,
				DigitalEnableJoin = 813
			};

			m_CountDownTimerDecrementButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 811,
				DigitalEnableJoin = 813
			};

			m_DefaultDeviceList = new VtProSubpageReferenceList(701, panel as IPanelDevice, m_Subpage)
            {
				MaxSize = 20,
				DigitalEnableJoin = 813,
				DigitalJoinIncrement = 1,
				SerialJoinIncrement = 2
            };

			m_CountDownSecondsLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 809,
				DigitalEnableJoin = 813
			};

			m_EnableZeroTouchToggleButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 812
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;

			yield return m_CountDownTimerIncrementButton;
			yield return m_CountDownTimerDecrementButton;

			yield return m_DefaultDeviceList;

			yield return m_CountDownSecondsLabel;

			yield return m_EnableZeroTouchToggleButton;
		}
	}
}