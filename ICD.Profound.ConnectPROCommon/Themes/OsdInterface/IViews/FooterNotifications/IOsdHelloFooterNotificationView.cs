namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.FooterNotifications
{
	public interface IOsdHelloFooterNotificationView : IOsdView
	{
		void SetLabelText(string text);

		void SetMainPageView(bool scheduler);
	}
}
