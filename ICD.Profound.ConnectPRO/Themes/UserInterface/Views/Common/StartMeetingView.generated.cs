using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class StartMeetingView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_StartMyMeetingButton;
		private VtProButton m_InstantMeetingButton;
		private VtProButton m_SettingsButton;
		private VtProButton m_RoomCombineButton;
		private VtProImageObject m_Logo;
		private VtProSubpageReferenceList m_ScheduleList;
		private VtProButton m_NoMeetingsButton;
		private VtProSimpleLabel m_NoMeetingsLabel;
		private VtProSimpleLabel m_SplashTimeLabel;

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
				DigitalVisibilityJoin = DEFAULT_SUBPAGE_VISIBILITY
			};

			m_StartMyMeetingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 91
			};

			m_InstantMeetingButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 700,
				DigitalEnableJoin = 701
			};

			m_SettingsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 115
			};

			m_RoomCombineButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 108,
				DigitalVisibilityJoin = 109
			};

			m_Logo = new VtProImageObject(panel, m_Subpage)
			{
				SerialGraphicsJoin = 30
			};

			m_ScheduleList = new VtProSubpageReferenceList(5, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 7,
				DigitalJoinIncrement = 2,
				SerialJoinIncrement = 5
			};

			m_NoMeetingsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				DigitalEnableJoin = 2
			};

			m_NoMeetingsLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 4
			};

			m_SplashTimeLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 8
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_StartMyMeetingButton;
			yield return m_InstantMeetingButton;
			yield return m_SettingsButton;
			yield return m_RoomCombineButton;
			yield return m_Logo;
			yield return m_ScheduleList;
			yield return m_NoMeetingsButton;
			yield return m_NoMeetingsLabel;
			yield return m_SplashTimeLabel;
		}
	}
}
