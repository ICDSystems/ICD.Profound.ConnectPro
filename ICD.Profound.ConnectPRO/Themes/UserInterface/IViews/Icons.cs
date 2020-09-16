using System;
using System.Collections.Generic;
using ICD.Profound.ConnectPROCommon.SettingsTree;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews
{
	public static class Icons
	{
		private const string PREFIX_ICON = "icon_";
		private const string PREFIX_SOURCE = "icon_";
		private const string PREFIX_DISPLAY = "icon_display_";

		private const string SUFFIX_STANDARDBLUE = "_stndBlue";
		private const string SUFFIX_GRAY = "_gray";
		private const string SUFFIX_LIGHTBLUE = "_lightBlue";
		private const string SUFFIX_RED = "_red";
		private const string SUFFIX_WHITE = "_white";
		private const string SUFFIX_YELLOW = "_yellow";

		private const string SOURCE_BLANK = "blank_sources";
		private const string DISPLAY_BLANK = "blank";
		private const string ICON_BLANK = "blank";

		/// <summary>
		/// Gets the icon for the given color state.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string GetIcon(string icon, eIconColor color)
		{
			icon = string.IsNullOrEmpty(icon) ? ICON_BLANK : icon;
			string suffix = GetIconColorSuffix(color);

			return string.Format("{0}{1}{2}", PREFIX_ICON, icon, suffix);
		}

		/// <summary>
		/// Gets the display icon for the given display color state.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string GetDisplayIcon(string icon, eDisplayColor color)
		{
			icon = string.IsNullOrEmpty(icon) ? DISPLAY_BLANK : icon;
			string suffix = GetDisplayColorSuffix(color);

			return string.Format("{0}{1}{2}", PREFIX_DISPLAY, icon, suffix);
		}

		/// <summary>
		/// Builds the icon string from the given base icon and color.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string GetSourceIcon(string icon, eSourceColor color)
		{
			icon = string.IsNullOrEmpty(icon) ? SOURCE_BLANK : icon;
			string suffix = GetSourceColorSuffix(color);

			// Edge case - blank source doesn't have a color
			if (icon == SOURCE_BLANK)
				suffix = string.Empty;

			return string.Format("{0}{1}{2}", PREFIX_SOURCE, icon, suffix);
		}

		/// <summary>
		/// Gets the icon color suffix for the given icon color mode.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private static string GetIconColorSuffix(eIconColor color)
		{
			switch (color)
			{
				case eIconColor.Grey:
					return SUFFIX_GRAY;
				case eIconColor.LightBlue:
					return SUFFIX_LIGHTBLUE;
				case eIconColor.Red:
					return SUFFIX_RED;
				case eIconColor.StandardBlue:
					return SUFFIX_STANDARDBLUE;
				case eIconColor.White:
					return SUFFIX_WHITE;
				case eIconColor.Yellow:
					return SUFFIX_YELLOW;
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}

		/// <summary>
		/// Gets the source icon color suffix for the given source color mode.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private static string GetSourceColorSuffix(eSourceColor color)
		{
			switch (color)
			{
				case eSourceColor.White:
					return SUFFIX_STANDARDBLUE;

				case eSourceColor.Grey:
					return SUFFIX_GRAY;

				case eSourceColor.Yellow:
					return SUFFIX_YELLOW;

				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}

		/// <summary>
		/// Gets the display icon color suffix for the given display color mode.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		private static string GetDisplayColorSuffix(eDisplayColor color)
		{
			switch (color)
			{
				case eDisplayColor.Grey:
					return SUFFIX_GRAY;
				case eDisplayColor.Yellow:
					return SUFFIX_YELLOW;

				case eDisplayColor.White:
				case eDisplayColor.Green:
					return SUFFIX_STANDARDBLUE;
				
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}

		private static readonly Dictionary<eSettingsIcon, string> s_SettingsIconNames =
			new Dictionary<eSettingsIcon, string>
			{
				{eSettingsIcon.Accounting, "accounting"},
				{eSettingsIcon.AddressBook, "addressBook"},
				{eSettingsIcon.Admin, "admin"},
				{eSettingsIcon.Airplay, "airplay"},
				{eSettingsIcon.Alarm, "alarm"},
				{eSettingsIcon.Alert, "alert"},
				{eSettingsIcon.Amfm, "amfm"},
				{eSettingsIcon.AudioCall, "audioCall"},
				{eSettingsIcon.AudioConference, "audioConference"},
				{eSettingsIcon.AudioInput, "audioInput"},
				{eSettingsIcon.Backgrounds, "backgrounds"},
				{eSettingsIcon.Benefits, "benefits"},
				{eSettingsIcon.Bluetooth, "bluetooth"},
				{eSettingsIcon.Bluray, "bluray"},
				{eSettingsIcon.Bolt, "bolt"},
				{eSettingsIcon.BottomPage, "bottomPage"},
				{eSettingsIcon.BrightnessMax, "brightnessMax"},
				{eSettingsIcon.BrightnessMed, "brightnessMed"},
				{eSettingsIcon.BrightnessMin, "brightnessMin"},
				{eSettingsIcon.Brush, "brush"},
				{eSettingsIcon.CableTV, "cableTV"},
				{eSettingsIcon.Calendar, "calendar"},
				{eSettingsIcon.Callout, "callout"},
				{eSettingsIcon.Camera, "camera"},
				{eSettingsIcon.Cart, "cart"},
				{eSettingsIcon.Climate, "climate"},
				{eSettingsIcon.Clock, "clock"},
				{eSettingsIcon.Close, "close"},
				{eSettingsIcon.CloseALT, "closeALT"},
				{eSettingsIcon.Collapse, "collapse"},
				{eSettingsIcon.Comment, "comment"},
				{eSettingsIcon.Commercial, "commercial"},
				{eSettingsIcon.Computer, "computer"},
				{eSettingsIcon.Conference, "conference"},
				{eSettingsIcon.ConferenceCamera, "conferenceCamera"},
				{eSettingsIcon.Confirm, "confirm"},
				{eSettingsIcon.ConfirmComment, "confirmComment"},
				{eSettingsIcon.Connector, "connector"},
				{eSettingsIcon.ConnectorPlate, "connectorPlate"},
				{eSettingsIcon.Contrast, "contrast"},
				{eSettingsIcon.Cue, "cue"},
				{eSettingsIcon.Dashboard, "dashboard"},
				{eSettingsIcon.Delete, "delete"},
				{eSettingsIcon.Deploy, "deploy"},
				{eSettingsIcon.Design, "design"},
				{eSettingsIcon.DigitalSignage, "digitalSignage"},
				{eSettingsIcon.Directory, "directory"},
				{eSettingsIcon.Disk, "disk"},
				{eSettingsIcon.Display, "display"},
				{eSettingsIcon.DocCam, "docCam"},
				{eSettingsIcon.Door, "door"},
				{eSettingsIcon.Download, "download"},
				{eSettingsIcon.DownloadCloud, "downloadCloud"},
				{eSettingsIcon.Drapes, "drapes"},
				{eSettingsIcon.Dvd, "dvd"},
				{eSettingsIcon.Dvr, "dvr"},
				{eSettingsIcon.Eject, "eject"},
				{eSettingsIcon.EndMeeting, "endMeeting"},
				{eSettingsIcon.Environment, "environment"},
				{eSettingsIcon.Eraser, "eraser"},
				{eSettingsIcon.Exit, "exit"},
				{eSettingsIcon.Expand, "expand"},
				{eSettingsIcon.Facebook, "facebook"},
				{eSettingsIcon.Fastforward, "fastforward"},
				{eSettingsIcon.Favorites, "favorites"},
				{eSettingsIcon.Features, "features"},
				{eSettingsIcon.Fire, "fire"},
				{eSettingsIcon.Fireplace, "fireplace"},
				{eSettingsIcon.FirstStep, "firstStep"},
				{eSettingsIcon.Flexible, "flexible"},
				{eSettingsIcon.Folder, "folder"},
				{eSettingsIcon.Game, "game"},
				{eSettingsIcon.Garage, "garage"},
				{eSettingsIcon.GenericAudio, "genericAudio"},
				{eSettingsIcon.GenericVideo, "genericVideo"},
				{eSettingsIcon.Globe, "globe"},
				{eSettingsIcon.Government, "government"},
				{eSettingsIcon.Graph, "graph"},
				{eSettingsIcon.Grid, "grid"},
				{eSettingsIcon.Hangup, "hangup"},
				{eSettingsIcon.HangupALT, "hangupALT"},
				{eSettingsIcon.Hd, "hd"},
				{eSettingsIcon.Hdmi, "hdmi"},
				{eSettingsIcon.Heart, "heart"},
				{eSettingsIcon.Help, "help"},
				{eSettingsIcon.History, "history"},
				{eSettingsIcon.Home, "home"},
				{eSettingsIcon.HotTub, "hotTub"},
				{eSettingsIcon.HouseAudio, "houseAudio"},
				{eSettingsIcon.Image, "image"},
				{eSettingsIcon.Info, "info"},
				{eSettingsIcon.Input, "input"},
				{eSettingsIcon.Instagram, "instagram"},
				{eSettingsIcon.Install, "install"},
				{eSettingsIcon.InstantMeeting, "instantMeeting"},
				{eSettingsIcon.InstantMeetingTouchCUE, "instantMeetingTouchCUE"},
				{eSettingsIcon.Internet, "internet"},
				{eSettingsIcon.InternetRadio, "internetRadio"},
				{eSettingsIcon.IPod, "iPod"},
				{eSettingsIcon.JoinByID, "joinByID"},
				{eSettingsIcon.JoinMeeting, "joinMeeting"},
				{eSettingsIcon.JoinMeetingALT, "joinMeetingALT"},
				{eSettingsIcon.Keyboard, "keyboard"},
				{eSettingsIcon.Laptop, "laptop"},
				{eSettingsIcon.LeaveCall, "leaveCall"},
				{eSettingsIcon.Level, "level"},
				{eSettingsIcon.Lights, "lights"},
				{eSettingsIcon.Location, "location"},
				{eSettingsIcon.LocationMinus, "locationMinus"},
				{eSettingsIcon.LocationPlus, "locationPlus"},
				{eSettingsIcon.Loop, "loop"},
				{eSettingsIcon.MacPro, "macPro"},
				{eSettingsIcon.Marketing, "marketing"},
				{eSettingsIcon.MediaPlayer, "mediaPlayer"},
				{eSettingsIcon.MedicalCross, "medicalCross"},
				{eSettingsIcon.Mic, "mic"},
				{eSettingsIcon.MicMute, "micMute"},
				{eSettingsIcon.Mobile, "mobile"},
				{eSettingsIcon.Mouse, "mouse"},
				{eSettingsIcon.Move, "move"},
				{eSettingsIcon.Movies, "movies"},
				{eSettingsIcon.MultipleUsers, "multipleUsers"},
				{eSettingsIcon.MusicPlaylist, "musicPlaylist"},
				{eSettingsIcon.Navigation, "navigation"},
				{eSettingsIcon.Network, "network"},
				{eSettingsIcon.NetworkOperationsCenters, "networkOperationsCenters"},
				{eSettingsIcon.News, "news"},
				{eSettingsIcon.NoSignal, "noSignal"},
				{eSettingsIcon.Notifications, "notifications"},
				{eSettingsIcon.Outlet, "outlet"},
				{eSettingsIcon.Passion, "passion"},
				{eSettingsIcon.Pause, "pause"},
				{eSettingsIcon.Pen, "pen"},
				{eSettingsIcon.Phone, "phone"},
				{eSettingsIcon.Pin, "pin"},
				{eSettingsIcon.Play, "play"},
				{eSettingsIcon.PlayLibrary, "playLibrary"},
				{eSettingsIcon.PlayPause, "playPause"},
				{eSettingsIcon.Police, "police"},
				{eSettingsIcon.Pool, "pool"},
				{eSettingsIcon.Popcorn, "popcorn"},
				{eSettingsIcon.PopupHide, "popupHide"},
				{eSettingsIcon.PopupShow, "popupShow"},
				{eSettingsIcon.Power, "power"},
				{eSettingsIcon.Presentation, "presentation"},
				{eSettingsIcon.Previous, "previous"},
				{eSettingsIcon.PrivacyMuteOff, "privacyMuteOff"},
				{eSettingsIcon.PrivacyMuteOn, "privacyMuteOn"},
				{eSettingsIcon.Process, "process"},
				{eSettingsIcon.Products, "products"},
				{eSettingsIcon.ProfoundPOD, "profoundPOD"},
				{eSettingsIcon.ProfoundSimple, "profoundSimple"},
				{eSettingsIcon.Programming, "programming"},
				{eSettingsIcon.ProjectManagement, "projectManagement"},
				{eSettingsIcon.Projector, "projector"},
				{eSettingsIcon.ProjectorALT, "projectorALT"},
				{eSettingsIcon.ProjectorScreen, "projectorScreen"},
				{eSettingsIcon.RadioSignal, "radioSignal"},
				{eSettingsIcon.Recent, "recent"},
				{eSettingsIcon.Record, "record"},
				{eSettingsIcon.RecordingDevice, "recordingDevice"},
				{eSettingsIcon.Refresh, "refresh"},
				{eSettingsIcon.Repeat, "repeat"},
				{eSettingsIcon.Residential, "residential"},
				{eSettingsIcon.Reverse, "reverse"},
				{eSettingsIcon.Rewind, "rewind"},
				{eSettingsIcon.RoomALT, "roomALT"},
				{eSettingsIcon.RoomCombine, "roomCombine"},
				{eSettingsIcon.RoomOffline, "roomOffline"},
				{eSettingsIcon.RoomPrivate, "roomPrivate"},
				{eSettingsIcon.Route, "route"},
				{eSettingsIcon.RouteSummary, "routeSummary"},
				{eSettingsIcon.Sales, "sales"},
				{eSettingsIcon.Satellite, "satellite"},
				{eSettingsIcon.SatelliteALT, "satelliteALT"},
				{eSettingsIcon.ScrollDown, "scrollDown"},
				{eSettingsIcon.ScrollUp, "scrollUp"},
				{eSettingsIcon.Search, "search"},
				{eSettingsIcon.SecureVideoConference, "secureVideoConference"},
				{eSettingsIcon.Security, "security"},
				{eSettingsIcon.SecurityCameras, "securityCameras"},
				{eSettingsIcon.Server, "server"},
				{eSettingsIcon.Shades, "shades"},
				{eSettingsIcon.Shield, "shield"},
				{eSettingsIcon.Shuffle, "shuffle"},
				{eSettingsIcon.Shuffle1, "shuffle1"},
				{eSettingsIcon.ShuffleOff, "shuffleOff"},
				{eSettingsIcon.Skipbackward, "skipbackward"},
				{eSettingsIcon.Skipforward, "skipforward"},
				{eSettingsIcon.Speaker, "speaker"},
				{eSettingsIcon.Sprinkler, "sprinkler"},
				{eSettingsIcon.Startmeeting, "startmeeting"},
				{eSettingsIcon.Stop, "stop"},
				{eSettingsIcon.Stopwatch, "stopwatch"},
				{eSettingsIcon.StreamingVideo, "streamingVideo"},
				{eSettingsIcon.SubjectMatterExpert, "subjectMatterExpert"},
				{eSettingsIcon.SwipeLeft, "swipeLeft"},
				{eSettingsIcon.SwipeRight, "swipeRight"},
				{eSettingsIcon.System, "system"},
				{eSettingsIcon.SystemAdvanced, "systemAdvanced"},
				{eSettingsIcon.Tablet, "tablet"},
				{eSettingsIcon.Technology, "technology"},
				{eSettingsIcon.Temp, "temp"},
				{eSettingsIcon.TopPage, "topPage"},
				{eSettingsIcon.Touch, "touch"},
				{eSettingsIcon.TouchCue, "touchCue"},
				{eSettingsIcon.TouchFree, "touchFree"},
				{eSettingsIcon.Touchscreen, "touchscreen"},
				{eSettingsIcon.Twitter, "twitter"},
				{eSettingsIcon.Understand, "understand"},
				{eSettingsIcon.Upload, "upload"},
				{eSettingsIcon.Usb, "usb"},
				{eSettingsIcon.UserGroup, "userGroup"},
				{eSettingsIcon.Vcr, "vcr"},
				{eSettingsIcon.Vga, "vga"},
				{eSettingsIcon.VideoInput, "videoInput"},
				{eSettingsIcon.VideoOutput, "videoOutput"},
				{eSettingsIcon.VideoRefresh, "videoRefresh"},
				{eSettingsIcon.VideoSwitcher, "videoSwitcher"},
				{eSettingsIcon.Volume, "volume"},
				{eSettingsIcon.VolumeMax, "volumeMax"},
				{eSettingsIcon.VolumeMin, "volumeMin"},
				{eSettingsIcon.VolumeMute, "volumeMute"},
				{eSettingsIcon.VolumeSelect, "volumeSelect"},
				{eSettingsIcon.WakeSleep, "wakeSleep"},
				{eSettingsIcon.WakeSleepALT, "wakeSleepALT"},
				{eSettingsIcon.Weather, "weather"},
				{eSettingsIcon.Wifi, "wifi"},
				{eSettingsIcon.WirelessPresentation, "wirelessPresentation"},
				{eSettingsIcon.Wrench, "wrench"},
				{eSettingsIcon.ZoomRoom, "zoomRoom"}
			};

		private static readonly Dictionary<eSettingsColor, string> s_ColorToSuffix =
			new Dictionary<eSettingsColor, string>
			{
				{eSettingsColor.LightBlue, "blue"}, 
				{eSettingsColor.Gray, "gray"}, 
				{eSettingsColor.White, "white"}
			};

		/// <summary>
		/// Returns the color variation for the given icon.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="settingsColor"></param>
		/// <returns></returns>
		public static string GetSettingsIcon(eSettingsIcon icon, eSettingsColor settingsColor)
		{
			return string.Format("icon_set_{0}_{1}", s_SettingsIconNames[icon], s_ColorToSuffix[settingsColor]);
		}
	}
}
