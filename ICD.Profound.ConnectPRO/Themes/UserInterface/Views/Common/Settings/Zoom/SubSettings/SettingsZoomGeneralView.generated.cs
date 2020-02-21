using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Zoom.SubSettings
{
	public sealed partial class SettingsZoomGeneralView
	{
		private VtProSubpage m_Subpage;
		private VtProButton m_MuteParticipantsButton;
		private VtProButton m_MuteMyCameraButton;
		private VtProButton m_EnableRecordButton;
		private VtProButton m_EnableDialOutButton;

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
				DigitalVisibilityJoin = 951
			};

			m_MuteParticipantsButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 410
			};

			m_MuteMyCameraButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 413
			};

			m_EnableRecordButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 414
			};

			m_EnableDialOutButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 415
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_MuteParticipantsButton;
			yield return m_MuteMyCameraButton;
			yield return m_EnableRecordButton;
			yield return m_EnableDialOutButton;
		}
	}
}
