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
	}
}
