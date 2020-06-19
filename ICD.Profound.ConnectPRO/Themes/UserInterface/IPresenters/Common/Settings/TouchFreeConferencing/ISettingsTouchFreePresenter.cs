using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPROCommon.SettingsTree.TouchFreeConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.TouchFreeConferencing
{
	public delegate void SourcePressedCallback(object sender, ISource source);

	public interface ISettingsTouchFreePresenter : ISettingsNodeBasePresenter<ISettingsTouchFreeView, TouchFreeSettingsLeaf>
	{
		/// <summary>
		/// Raised when the user presses a source.
		/// </summary>
		event Sources.SourcePressedCallback OnSourcePressed;
	}
}