using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.HeaderNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.HeaderNotifications
{
	[ViewBinding(typeof(IOsdTouchFreeHeaderNotificationView))]
	public sealed partial class OsdTouchFreeHeaderNotificationView : AbstractOsdView, IOsdTouchFreeHeaderNotificationView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public OsdTouchFreeHeaderNotificationView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetTimer(int seconds)
		{
			m_Timer.SetLabelText(seconds.ToString());
		}
	}
}