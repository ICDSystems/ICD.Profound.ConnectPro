using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom.SubSettings;
using ICD.Profound.ConnectPROCommon.SettingsTree.Zoom;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Zoom.SubSettings
{
	public interface ISettingsZoomSubPresenter : IUiPresenter
	{
		/// <summary>
		/// Gets/sets the wrapped zoom settings leaf instance.
		/// </summary>
		ZoomSettingsLeaf Settings { get; set; }
	}

	public interface ISettingsZoomSubPresenter<TView> : IUiPresenter<TView>, ISettingsZoomSubPresenter
		where TView : ISettingsZoomSubView
	{
	}
}
