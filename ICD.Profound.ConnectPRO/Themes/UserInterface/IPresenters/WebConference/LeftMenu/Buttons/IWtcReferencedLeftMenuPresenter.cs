using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons
{
	public interface IWtcReferencedLeftMenuPresenter : IWtcPresenter<IWtcReferencedLeftMenuView>
	{
		/// <summary>
		/// Gets/sets the selection state of the button.
		/// </summary>
		bool Selected { get; set; }
	}
}
