using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu
{
	public enum eMode
	{
		Default,
		WebConference,
		CallOut
	}

	public interface IWtcLeftMenuPresenter : IWtcPresenter<IWtcLeftMenuView>
	{
		/// <summary>
		/// Gets/sets the current menu mode.
		/// </summary>
		eMode Mode { get; set; }
	}
}
