namespace ICD.Profound.ConnectPRO.Devices
{
	public static class ConnectProEventMessages
	{
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

		// Source routed
		public const string KEY_SOURCE_ROUTED = "source routed";
		public const string MESSAGE_SOURCE_ROUTED = "source routed";
		public const string MESSAGE_SOURCE_UNROUTED = "source unrouted";

		// Room combine
		public const string KEY_ROOM_COMBINED = "room combined";
		public const string MESSAGE_ROOM_COMBINED = "room combined";
		public const string MESSAGE_ROOM_UNCOMBINED = "room uncombined";

		// Privacy mute/unmute
		public const string KEY_PRIVACY_MUTE = "privacy mute";
		public const string MESSAGE_PRIVACY_MUTED = "privacy muted";
		public const string MESSAGE_PRIVACY_UNMUTED = "privacy unmuted";
	}
}
