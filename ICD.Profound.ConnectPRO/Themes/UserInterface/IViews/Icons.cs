using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews
{
	public static class Icons
	{
		private const string PREFIX_SOURCE = "icon_";
		private const string PREFIX_DISPLAY = "icon_display_";

		private const string SUFFIX_WHITE = "_stndBlue";//"_white";
		private const string SUFFIX_TWOTONE = "_gray";//"_twoTone";
		private const string SUFFIX_GREY = "_gray";
		private const string SUFFIX_YELLOW = "_yellow";

		private const string SOURCE_BLANK = "blank_sources";
		private const string DISPLAY_BLANK = "blank";

		/// <summary>
		/// Gets the display icon for the given display colour state.
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

			// Edge case - blank source doesn't have a colour
			if (icon == SOURCE_BLANK)
				suffix = string.Empty;

			return string.Format("{0}{1}{2}", PREFIX_SOURCE, icon, suffix);
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
					return SUFFIX_WHITE;

				case eSourceColor.Grey:
					return SUFFIX_TWOTONE;

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
					return SUFFIX_GREY;
				case eDisplayColor.Yellow:
					return SUFFIX_YELLOW;

				case eDisplayColor.White:
				case eDisplayColor.Green:
					return SUFFIX_WHITE;
				
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}
	}
}
