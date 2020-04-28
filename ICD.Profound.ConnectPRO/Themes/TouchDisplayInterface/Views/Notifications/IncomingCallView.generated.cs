using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Notifications
{
	public sealed partial class IncomingCallView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_IncomingCallLabel;
		private VtProImageObject m_Icon;
		private VtProAdvancedButton m_AnswerButton;
		private VtProButton m_RejectButton;
		private VtProSound m_Ringtone;

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
				DigitalVisibilityJoin = 300
			};

			m_IncomingCallLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 303
			};

			m_Icon = new VtProImageObject(panel, m_Subpage)
			{
				SerialGraphicsJoin = 301
			};

			m_AnswerButton = new VtProAdvancedButton(panel, m_Subpage)
			{
				AnalogModeJoin = 301,
				DigitalPressJoin = 301
			};

			m_RejectButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 302,
				DigitalVisibilityJoin = 303
			};

			m_Ringtone = new VtProSound(panel as IPanelDevice)
			{
				JoinNumber = 304,
				StopSoundJoin = 305
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_IncomingCallLabel;
			yield return m_Icon;
			yield return m_AnswerButton;
			yield return m_RejectButton;
		}
	}
}