using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews
{
	public enum eDisplayColor
	{
		Grey,
		Yellow,
		Green
	}

	public enum eSourceColor
	{
		White,
		Grey,
		Yellow,
		Green
	}

	public static class Colors
	{
		private const string COLOR_WHITE = "#FFFFFF";
		private const string COLOR_LIGHT_GREY = "#8B8B8B";
		private const string COLOR_DARK_GREY = "#535353";

		/// <summary>
		/// Gets the hex color code for the display text in the current color mode.
		/// </summary>
		/// <param name="displayColor"></param>
		/// <returns></returns>
		public static string DisplayColorToTextColor(eDisplayColor displayColor)
		{
			switch (displayColor)
			{
				case eDisplayColor.Grey:
					return COLOR_LIGHT_GREY;
				case eDisplayColor.Yellow:
					return COLOR_DARK_GREY;
				case eDisplayColor.Green:
					return COLOR_WHITE;

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
				case eSourceColor.Green:
					return COLOR_WHITE;

				default:
					throw new ArgumentOutOfRangeException("sourceColor");
			}
		}

		public static eSourceColor DisplayColorToSourceColor(eDisplayColor displayColor)
		{
			switch (displayColor)
			{
				case eDisplayColor.Grey:
					return eSourceColor.Grey;
				case eDisplayColor.Yellow:
					return eSourceColor.Yellow;
				case eDisplayColor.Green:
					return eSourceColor.Green;

				default:
					throw new ArgumentOutOfRangeException("displayColor");
			}
		}
	}
}
