using ICD.Common.Properties;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Cameras;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Cameras
{
	public interface ICameraButtonsPresenter : IPopupPresenter<ICameraButtonsView>
	{
		/// <summary>
		/// Sets the conference control that is currently active.
		/// </summary>
		/// <param name="value"></param>
		void SetActiveConferenceControl([CanBeNull] IConferenceDeviceControl value);

		/// <summary>
		/// Returns true if any of the child features (layout, active camera, etc) are currently available.
		/// </summary>
		bool AnyFeaturesAvailable { get; }
	}
}
