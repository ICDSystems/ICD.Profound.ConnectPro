namespace ICD.Profound.ConnectPROCommon.Devices
{
	public static class ConnectProEventMessages
	{
		// Zoom Room Controls example
		/*
		"rules": {
			"meeting_started": ["meeting:in meeting"],
			"meeting_stopped": ["meeting:out of meeting"],
			"microphone_muted": ["privacy mute:privacy muted"],
			"microphone_unmuted": ["privacy mute:privacy unmuted"],
			"video_started": ["camera privacy mute:privacy unmuted"],
			"video_stopped": ["camera privacy mute:privacy muted"],
			"operation_time_started": ["wake:wake"],
			"operation_time_ended": ["wake:sleep"]
		}
		*/

		// Keys are completely arbitrary, but unique

		// In/out of meeting
		public const string KEY_MEETING = "meeting";
		public const string MESSAGE_OUT_OF_MEETING = "out of meeting";
		public const string MESSAGE_IN_MEETING = "in meeting";

		// Wake/sleep
		public const string KEY_WAKE = "wake";
		public const string MESSAGE_SLEEP = "sleep";
		public const string MESSAGE_WAKE = "wake";

		// In/out of call
		public const string KEY_CALL = "call";
		public const string MESSAGE_IN_CALL = "in call";
		public const string MESSAGE_OUT_OF_CALL = "out of call";

		// Incoming call
		public const string KEY_INCOMING_CALL = "incoming call";
		public const string MESSAGE_INCOMING_CALL = "incoming call";
		public const string MESSAGE_NO_INCOMING_CALL = "no incoming call";

		// Room combine
		public const string KEY_ROOM_COMBINED = "room combined";
		public const string MESSAGE_ROOM_COMBINED = "room combined";
		public const string MESSAGE_ROOM_UNCOMBINED = "room uncombined";

		// Privacy mute/unmute
		public const string KEY_PRIVACY_MUTE = "privacy mute";
		public const string MESSAGE_PRIVACY_MUTED = "privacy muted";
		public const string MESSAGE_PRIVACY_UNMUTED = "privacy unmuted";

		// Camera privacy mute/unmute
		public const string KEY_CAMERA_PRIVACY_MUTE = "camera privacy mute";
		public const string MESSAGE_CAMERA_PRIVACY_MUTED = "privacy muted";
		public const string MESSAGE_CAMERA_PRIVACY_UNMUTED = "privacy unmuted";

		// Lists
		public const string KEY_AUDIO_SOURCES = "audio sources";
		public const string KEY_VIDEO_SOURCES = "video sources";
		public const string KEY_ACTIVE_CAMERA = "active camera";
	}
}
