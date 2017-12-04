using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources
{
	public interface ISourceSelectPresenter<TView> : IPresenter<TView>
		where TView : ISourceSelectView
	{
	}
}
