using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
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
	}
}