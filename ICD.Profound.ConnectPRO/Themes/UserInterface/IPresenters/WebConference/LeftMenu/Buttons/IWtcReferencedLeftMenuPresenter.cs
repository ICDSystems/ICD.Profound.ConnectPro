using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons
{
	public interface IWtcReferencedLeftMenuPresenter : IWtcPresenter<IWtcReferencedLeftMenuView>
	{
		/// <summary>
		/// Hides the child subpage/s managed by this button.
		/// </summary>
		void HideSubpages();
	}
}
