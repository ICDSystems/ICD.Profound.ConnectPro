namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews
{
	public interface IHelloView : ITouchDisplayView
	{
		void SetLabelText(string text);

		void SetMainPageView(bool scheduler);
	}
}