using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu
{
	public interface IWtcReferencedLeftMenuPresenter : IUiPresenter<IWtcReferencedLeftMenuView>
	{
		/// <summary>
		/// Gets/sets the model for this child item.
		/// </summary>
		IWtcLeftMenuButtonModel ButtonModel { get; set; }
	}
}