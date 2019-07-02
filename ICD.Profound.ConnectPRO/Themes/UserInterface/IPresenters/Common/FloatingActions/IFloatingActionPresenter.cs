using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions
{
	public interface IFloatingActionPresenter<TView> : IUiPresenter<TView>
		where TView : IFloatingActionView
	{
	}
}
