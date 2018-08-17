using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Options;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Options
{
	public interface IOptionPresenter<TView> : IPresenter<TView>
		where TView : IOptionView
	{
	}
}
