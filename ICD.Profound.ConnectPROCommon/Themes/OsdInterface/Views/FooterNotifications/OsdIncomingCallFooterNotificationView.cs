using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.FooterNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.FooterNotifications
{
	[ViewBinding(typeof(IOsdIncomingCallFooterNotificationView))]
	public sealed partial class OsdIncomingCallFooterNotificationView : AbstractOsdView, IOsdIncomingCallFooterNotificationView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public OsdIncomingCallFooterNotificationView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
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

		public void PlayRingtone(bool playing)
		{
			if (playing)
				m_Ringtone.Play(3300);
			else
				m_Ringtone.Stop();
		}
	}
}