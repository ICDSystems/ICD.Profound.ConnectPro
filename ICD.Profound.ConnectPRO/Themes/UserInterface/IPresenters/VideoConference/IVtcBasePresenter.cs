using System;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference
{
	public interface IVtcBasePresenter : IPopupPresenter<IVtcBaseView>
	{
		ITraditionalConferenceDeviceControl ActiveConferenceControl { get; }

		event EventHandler OnActiveConferenceControlChanged;
	}
}
