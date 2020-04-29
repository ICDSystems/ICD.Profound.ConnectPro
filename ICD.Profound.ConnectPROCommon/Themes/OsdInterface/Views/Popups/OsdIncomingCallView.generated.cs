using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Popups
{
	public sealed partial class OsdIncomingCallView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_SourceNameLabel;
		private VtProSimpleLabel m_IncomingCallLabel;
		private VtProImageObject m_Icon;
		private VtProAdvancedButton m_Background;
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

			m_SourceNameLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 302
			};

			m_IncomingCallLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 303
			};

			m_Icon = new VtProImageObject(panel, m_Subpage)
			{
				SerialGraphicsJoin = 301
			};

			m_Background = new VtProAdvancedButton(panel, m_Subpage)
			{
				AnalogModeJoin = 301
			};

			m_Ringtone = new VtProSound(panel as IPanelDevice)
			{
				JoinNumber = 13,
				StopSoundJoin = 14
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_SourceNameLabel;
			yield return m_IncomingCallLabel;
			yield return m_Icon;
			yield return m_Background;
		}
	}
}