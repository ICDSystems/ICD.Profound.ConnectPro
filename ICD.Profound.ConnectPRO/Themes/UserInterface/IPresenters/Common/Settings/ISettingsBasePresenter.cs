using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings
{
	public interface ISettingsBasePresenter : IPopupPresenter<ISettingsBaseView>
	{

		/// <summary>
		/// Tracks if the user has logged into the settings pages
		/// </summary>
		bool IsLoggedIn { get; set; }
	}
}
