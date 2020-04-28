namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews
{
	public interface IHelloView : ITouchDisplayView
	{
		void SetLabelText(string text);

		void SetMainPageView(bool scheduler);
	}
}