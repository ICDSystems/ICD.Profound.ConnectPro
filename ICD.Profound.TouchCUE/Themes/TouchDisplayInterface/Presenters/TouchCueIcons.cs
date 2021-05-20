using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.SettingsTree;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Presenters
{
	public static class TouchCueIcons
	{
		private static readonly Dictionary<eSettingsIcon, eTouchCueIcon> s_SettingsIcons =
			new Dictionary<eSettingsIcon, eTouchCueIcon>
			{
				{eSettingsIcon.Admin, eTouchCueIcon.Settings},
				{eSettingsIcon.Backgrounds, eTouchCueIcon.ConnectPro},
				{eSettingsIcon.Clock, eTouchCueIcon.Clock},
				{eSettingsIcon.Conference, eTouchCueIcon.VideoConference},
				{eSettingsIcon.Cue, eTouchCueIcon.TouchCue},
				{eSettingsIcon.Directory, eTouchCueIcon.Directory},
				{eSettingsIcon.Pin, eTouchCueIcon.Security},
				{eSettingsIcon.WakeSleep, eTouchCueIcon.WakeSleep},
			};

		public static string GetIcon(eTouchCueIcon icon)
		{
			return GetIcon(icon, eTouchCueColor.White);
		}

		public static string GetIcon(eTouchCueIcon icon, eTouchCueColor color)
		{
			return string.Format("ic_{0}_{1}", icon.ToString().ToLower(), color.ToString().ToLower());
		}

		public static string GetSourceIcon(ISource source, eTouchCueColor color)
		{
			eTouchCueIcon icon;
			ConnectProSource connectProSource = source as ConnectProSource;
			if (connectProSource != null && connectProSource.Icon != null && EnumUtils.TryParse(connectProSource.Icon, true, out icon))
				return GetIcon(icon, color);
			return GetIcon(source.ConnectionType.HasFlag(eConnectionType.Video)
				? eTouchCueIcon.GenericVideo
				: eTouchCueIcon.GenericAudio, color);
		}

		public static string GetSettingsIcon(eSettingsIcon icon, eTouchCueColor color)
		{
			eTouchCueIcon mappedIcon;
			if (!s_SettingsIcons.TryGetValue(icon, out mappedIcon))
				mappedIcon = eTouchCueIcon.Settings;
			return GetIcon(mappedIcon, color);
		}
	}

	public enum eTouchCueColor
	{
		White,
		Gray,
		LightBlue,
		Red,
		Green
	}

	public enum eTouchCueIcon
	{
		Accounting,
		Advance,
		Alarm,
		AmFm,
		Answer,
		ArrowDown,
		ArrowLeft,
		ArrowRight,
		ArrowUp,
		AudioCall,
		AudioConference,
		AudioInput,
		Bathroom,
		Benefits,
		Bluetooth,
		Bluray,
		BottomPage,
		Calendar,
		CallOut,
		Climate,
		Clock,
		Close,
		Collapse,
		Commercial,
		Computer,
		ConferenceCameraAlt,
		ConferenceCamera,
		Conference,
		Confirm,
		ConnectPro,
		Cue,
		Delete,
		Deploy,
		Design,
		DigitalSignage,
		Directory,
		Display,
		DocCam,
		Dvd,
		Environment,
		Exit,
		Expand,
		FastForward,
		Favorites,
		Features,
		FirstStep,
		Flexible,
		Game,
		GenericAudio,
		GenericVideo,
		Government,
		HangUpAlt,
		HangUp,
		Hdmi,
		Hide,
		History,
		Home,
		HouseAudio,
		Input,
		Install,
		InternetRadio,
		Ipod,
		JoinMeetingAlt,
		JoinMeeting,
		Laptop,
		LeaveCall,
		Level,
		Lights,
		List,
		Loop,
		MacPro,
		Marketing,
		MediaPlayer,
		Mic,
		Mobile,
		Movies,
		MultipleUsers,
		MusicNote,
		NetworkOperationCenters,
		Network,
		NoSignal,
		Passion,
		Pause,
		Play,
		Pod,
		Popcorn,
		Power,
		Presentation,
		PrivacyMuteOff,
		PrivacyMuteOn,
		Process,
		Products,
		ProfoundSimple,
		Programming,
		ProjectManagement,
		ProjectorAlt,
		Projector,
		RecordingDevice,
		Record,
		Remote,
		Repeat,
		Residential,
		Reveal,
		Rewind,
		RoomPrivate,
		Room,
		Sales,
		SatelliteAlt,
		Satellite,
		Screen,
		ScrollDown,
		ScrollUp,
		Search,
		SecureVideoConference,
		SecurityCamera,
		Security,
		Server,
		Settings,
		Shades,
		Share,
		SkipBack,
		SkipForward,
		Speaker,
		Stop,
		StreamingVideo,
		SubjectMatterExpert,
		SwipeLeft,
		SwipeRight,
		Tag,
		Technology,
		Temp,
		ThumbsUp,
		TopPage,
		TouchCue,
		Touchscreen,
		Touch,
		TvRemote,
		Understand,
		UserGroup,
		VgaAudio,
		Vga,
		VideoConference,
		VideoRefresh,
		VideoStreaming,
		VideoSwitcher,
		VolumeDown,
		VolumeGeneric,
		VolumeList,
		VolumeMax,
		VolumeMin,
		VolumeMute,
		VolumeSelect,
		VolumeUp,
		Volume,
		WakeSleep,
		Wand,
		Warehouse,
		Weather,
		Wifi,
		WirelessPresentation,
		ZoomIn,
		ZoomOut,
		ZoomRoom,
		Zoom
	}
}
