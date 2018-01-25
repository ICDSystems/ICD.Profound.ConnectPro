using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews
{
	public static class Icons
	{
		private const string DISPLAY_GREY = "display_blank_gray";
		private const string DISPLAY_WHITE = "display_blank_white";
		private const string DISPLAY_YELLOW = "display_blank_yellow";

		private const string SUFFIX_WHITE = "_white";
		private const string SUFFIX_TWOTONE = "_twoTone";

		/// <summary>
		/// Blank icon state.
		/// </summary>
		public const string SOURCE_BLANK = "icon_blank_sources";

		/// <summary>
		/// Gets the display icon for the given display colour state.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string GetDisplayIcon(eDisplayColor color)
		{
			switch (color)
			{
				case eDisplayColor.Grey:
					return DISPLAY_GREY;
				case eDisplayColor.Yellow:
					return DISPLAY_YELLOW;
				case eDisplayColor.Green:
					return DISPLAY_WHITE;
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}

		/// <summary>
		/// Builds the icon string from the given base icon and color.
		/// </summary>
		/// <param name="icon"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string GetSourceIcon(string icon, eSourceColor color)
		{
			string suffix = GetSourceColorSuffix(color);
			return string.Format("{0}{1}", icon, suffix);
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
				case eSourceColor.Grey:
				case eSourceColor.Yellow:
					return SUFFIX_TWOTONE;
				case eSourceColor.Green:
					return SUFFIX_WHITE;
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}
	}
}
