using ICD.Common.Properties;
using ICD.Profound.ConnectPRO.SettingsTree;
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

		/// <summary>
		/// Gets the current root settings node item.
		/// </summary>
		[CanBeNull]
		IRootSettingsNode RootNode { get; }

		/// <summary>
		/// Navigates to the given settings node.
		/// </summary>
		/// <param name="node"></param>
		void NavigateTo([NotNull] ISettingsNodeBase node);
	}
}
