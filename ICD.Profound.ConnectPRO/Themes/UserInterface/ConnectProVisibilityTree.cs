using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.AudioConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.FloatingActions;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Administrative;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Conferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.CUE;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.RoomCombine;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.CableTv;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.ActiveCalls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Contacts;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference.Dtmf;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.ActiveMeeting;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.Contacts;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface
{
	/// <summary>
	/// The rules for view visibility, e.g. prevent certain items from being visible at the same time.
	/// </summary>
	public sealed class ConnectProVisibilityTree
	{
		private readonly IVisibilityNode m_RootVisibility;

		public ConnectProVisibilityTree(INavigationController navigationController)
		{
			// Only allow one of the start/end buttons to be visible at any given time
			m_RootVisibility = new DefaultVisibilityNode(navigationController.LazyLoadPresenter<IStartMeetingPresenter>());

			IVisibilityNode displaysVisibility = new SingleVisibilityNode();
			displaysVisibility.AddPresenter(navigationController.LazyLoadPresenter<IMenuCombinedSimpleModePresenter>());
			displaysVisibility.AddPresenter(navigationController.LazyLoadPresenter<IMenuCombinedAdvancedModePresenter>());
			displaysVisibility.AddPresenter(navigationController.LazyLoadPresenter<IMenu2DisplaysPresenter>());
			displaysVisibility.AddPresenter(navigationController.LazyLoadPresenter<IMenu3PlusDisplaysPresenter>());

			m_RootVisibility.AddNode(displaysVisibility);

			// Video Conference node
			IVisibilityNode videoConferencingVisibility = new SingleVisibilityNode();
			videoConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcContactsNormalPresenter>());
			videoConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcContactsPolycomPresenter>());
			videoConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcSharePresenter>());
			videoConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcDtmfPresenter>());
			videoConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcActiveCallsPresenter>());

			// Web Conference node
			IVisibilityNode webConferencingVisibility = new SingleVisibilityNode();
			webConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWtcCallOutPresenter>());
			webConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWtcSharePresenter>());
			webConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWtcContactListPresenter>());
			webConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWtcRecordingPresenter>());
			webConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWtcActiveMeetingPresenter>());
			webConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWtcStartMeetingPresenter>());

			// Audio Conference node
			IVisibilityNode audioConferencingVisibility = new SingleVisibilityNode();
			audioConferencingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IAtcBasePresenter>());

			// Meeting node
			IVisibilityNode meetingVisibility = new VisibilityNode();

			meetingVisibility.AddNode(videoConferencingVisibility);
			meetingVisibility.AddNode(audioConferencingVisibility);
			meetingVisibility.AddNode(webConferencingVisibility);

			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IEndMeetingPresenter>());
			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IFloatingActionPrivacyMutePresenter>());
			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IFloatingActionVolumePresenter>());
			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IFloatingActionCameraPresenter>());
			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcBasePresenter>());
			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWtcBasePresenter>());
			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<ICableTvPresenter>());
			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWebConferencingAlertPresenter>());
			meetingVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWebConferencingStepPresenter>());

			m_RootVisibility.AddNode(meetingVisibility);

			// Camera visibility
			IVisibilityNode cameraVisibility = new SingleVisibilityNode();
			cameraVisibility.AddPresenter(navigationController.LazyLoadPresenter<ICameraControlPresenter>());
			cameraVisibility.AddPresenter(navigationController.LazyLoadPresenter<ICameraActivePresenter>());
			cameraVisibility.AddPresenter(navigationController.LazyLoadPresenter<IWtcLeftMenuPresenter>());
			cameraVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcButtonListPresenter>());
			cameraVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcContactsNormalPresenter>());
			cameraVisibility.AddPresenter(navigationController.LazyLoadPresenter<IVtcContactsPolycomPresenter>());

			// Settings node
			IVisibilityNode settingsVisibility = new SingleVisibilityNode();
			settingsVisibility.AddPresenter(navigationController.LazyLoadPresenter<ISettingsClockPresenter>());
			settingsVisibility.AddPresenter(navigationController.LazyLoadPresenter<ISettingsPinPresenter>());
			settingsVisibility.AddPresenter(navigationController.LazyLoadPresenter<ISettingsPowerPresenter>());
			settingsVisibility.AddPresenter(navigationController.LazyLoadPresenter<ISettingsDirectoryPresenter>());
			settingsVisibility.AddPresenter(navigationController.LazyLoadPresenter<ISettingsCueBackgroundPresenter>());
			settingsVisibility.AddPresenter(navigationController.LazyLoadPresenter<ISettingsRoomCombinePresenter>());

			m_RootVisibility.AddNode(settingsVisibility);

			// These presenters are initially visible.
			navigationController.NavigateTo<IHeaderPresenter>();
			navigationController.NavigateTo<IHardButtonsPresenter>();

			// These presenters control their own visibility.
			navigationController.LazyLoadPresenter<IEndMeetingPresenter>();
			navigationController.LazyLoadPresenter<IStartMeetingPresenter>();
			navigationController.LazyLoadPresenter<IFloatingActionPrivacyMutePresenter>();
			navigationController.LazyLoadPresenter<IFloatingActionVolumePresenter>();
			navigationController.LazyLoadPresenter<IFloatingActionCameraPresenter>();
			navigationController.LazyLoadPresenter<IVtcCallListTogglePresenter>();
			navigationController.LazyLoadPresenter<IVtcIncomingCallPresenter>();
			navigationController.LazyLoadPresenter<IAtcIncomingCallPresenter>();
		}
	}
}
