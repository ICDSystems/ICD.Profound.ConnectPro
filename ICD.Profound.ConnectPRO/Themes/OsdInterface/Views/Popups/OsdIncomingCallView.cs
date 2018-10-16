using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Popups
{
	public sealed partial class OsdIncomingCallView : AbstractOsdView, IOsdIncomingCallView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public OsdIncomingCallView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetIcon(string icon)
		{
			m_Icon.SetImageUrl(icon);
		}

		public void SetSourceName(string name)
		{
			m_SourceNameLabel.SetLabelText(name);
		}

		public void SetCallerInfo(string number)
		{
			m_IncomingCallLabel.SetLabelText(number);
		}

		public void SetBackgroundMode(eOsdIncomingCallBackgroundMode mode)
		{
			m_Background.SetMode((ushort) mode);
		}
	}
}