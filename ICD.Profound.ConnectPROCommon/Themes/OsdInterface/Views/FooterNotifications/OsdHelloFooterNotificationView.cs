using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.FooterNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.FooterNotifications
{
	[ViewBinding(typeof(IOsdHelloFooterNotificationView))]
	public sealed partial class OsdHelloFooterNotificationView : AbstractOsdView, IOsdHelloFooterNotificationView
	{
		public OsdHelloFooterNotificationView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetLabelText(string text)
		{
			m_Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets whether the hello text is in the Main View (true, no scheduler and out of meeting)
		/// or in the notification section (false, scheduler or in meeting)
		/// </summary>
		/// <param name="mainPageView"></param>
		public void SetMainPageView(bool mainPageView)
		{
			// TODO: hack, replace when we make OSD specific controls
			m_Label.Enable(mainPageView);
		}
	}
}
