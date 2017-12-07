namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources
{
	public interface ISourceSelectDualView : ISourceSelectView
	{
		/// <summary>
		/// Sets the visibility of the arrows indicating there are more than 4 items.
		/// </summary>
		/// <param name="show"></param>
		void ShowArrows(bool show);
	}
}
