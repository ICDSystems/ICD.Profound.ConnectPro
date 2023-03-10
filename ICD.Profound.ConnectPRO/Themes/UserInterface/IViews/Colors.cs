using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews
{
	public enum eIconColor
	{
		Grey,
		LightBlue,
		Red,
		StandardBlue,
		White,
		Yellow
	}

	public enum eSettingsColor
	{
		LightBlue,
		Gray,
		White
	}

	public enum eDisplayColor
	{
		White, // Routed with no controls
		Grey, // Nothing routed/selected
		Yellow, // Currently routing
		Green // Routed with controls
	}

	public enum eSourceColor
	{
		White,
		Grey,
		Yellow
	}

	public static class Colors
	{
		public const string COLOR_DARK_BLUE = "#2998B0";
		public const string COLOR_WHITE = "#FFFFFF";
		public const string COLOR_LIGHT_GREY = "#8B8B8B";
		public const string COLOR_DARK_GREY = "#535353";
		public const string COLOR_RED = "#FB2F24";

		/// <summary>
		/// Gets the hex color code for the display text in the current color mode.
		/// </summary>
		/// <param name="displayColor"></param>
		/// <returns></returns>
		public static string DisplayColorToTextColor(eDisplayColor displayColor)
		{
			switch (displayColor)
			{
				case eDisplayColor.White:
				case eDisplayColor.Green:
					return COLOR_DARK_GREY;
				case eDisplayColor.Grey:
					return COLOR_LIGHT_GREY;
				case eDisplayColor.Yellow:
					return COLOR_DARK_GREY;

				default:
					throw new ArgumentOutOfRangeException("displayColor");
			}
		}

		/// <summary>
		/// Gets the hex color code for the source text in the current color mode.
		/// </summary>
		/// <param name="sourceColor"></param>
		/// <returns></returns>
		public static string SourceColorToTextColor(eSourceColor sourceColor)
		{
			switch (sourceColor)
			{
				case eSourceColor.White:
					return COLOR_DARK_GREY;
				case eSourceColor.Grey:
					return COLOR_LIGHT_GREY;
				case eSourceColor.Yellow:
					return COLOR_DARK_GREY;

				default:
					throw new ArgumentOutOfRangeException("sourceColor");
			}
		}
	}
}
