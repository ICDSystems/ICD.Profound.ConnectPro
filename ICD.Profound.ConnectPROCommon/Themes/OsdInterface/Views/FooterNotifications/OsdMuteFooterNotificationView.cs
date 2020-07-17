using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.FooterNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.FooterNotifications
{
	[ViewBinding(typeof(IOsdMuteFooterNotificationView))]
	public sealed partial class OsdMuteFooterNotificationView : AbstractOsdView, IOsdMuteFooterNotificationView
	{
		public OsdMuteFooterNotificationView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
