using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.SettingsTree
{
	public static class SettingsTreeIcons
	{
		public const string ICON_ADMIN = "icon_set_admin";
		public const string ICON_BACKGROUNDS = "icon_set_backgrounds";
		public const string ICON_CLOCK = "icon_set_clock";
		public const string ICON_CONFERENCE = "icon_set_conference";
		public const string ICON_CUE = "icon_set_cue";
		public const string ICON_DIRECTORY = "icon_set_directory";
		public const string ICON_GRID = "icon_set_grid";
		public const string ICON_NOTIFICATION = "icon_set_notification";
		public const string ICON_PHONE = "icon_set_phone";
		public const string ICON_PIN = "icon_set_pin";
		public const string ICON_POWER = "icon_set_power";
		public const string ICON_ROOM_COMBINE = "icon_set_roomCombine";
		public const string ICON_ZOOM = "icon_set_zoom";

		public enum eColor
		{
			Blue,
			Gray,
			White
		}

		private static readonly Dictionary<eColor, string> s_ColorToSuffix =
			new Dictionary<eColor, string>
			{
				{eColor.Blue, "blue"}, 
				{eColor.Gray, "gray"}, 
				{eColor.White, "white"}
			};

		/// <summary>
		/// Returns the color variation for the given icon.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string GetIcon(string icon, eColor color)
		{
			return string.Format("{0}_{1}", icon, s_ColorToSuffix[color]);
		}
	}
}
