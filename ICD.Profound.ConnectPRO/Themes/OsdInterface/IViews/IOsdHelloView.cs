namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews
{
	public interface IOsdHelloView : IOsdView
	{
		void SetLabelText(string text);

		void SetMainPageView(bool scheduler);
	}
}
