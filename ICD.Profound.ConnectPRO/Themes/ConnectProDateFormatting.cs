using System;
using System.Globalization;
using ICD.Common.Utils;
using ICD.Common.Utils.Globalization;

namespace ICD.Profound.ConnectPRO.Themes
{
	public static class ConnectProDateFormatting
	{
		private static CultureInfo s_ThemeCulture;

		#region Properties

		/// <summary>
		/// Gets the custom culture for presenting data values to the end user in ConnectPro.
		/// </summary>
		public static CultureInfo ThemeCulture
		{
			get
			{
				if (s_ThemeCulture == null)
				{
					CultureInfo copy = (CultureInfo)IcdCultureInfo.CurrentCulture.Clone();

					// Custom AM/PM
					copy.DateTimeFormat = (DateTimeFormatInfo)copy.DateTimeFormat.Clone();
					copy.DateTimeFormat.AMDesignator = "a";
					copy.DateTimeFormat.PMDesignator = "p";

					// Janky, remove the space between minutes and AM/PM
					copy.DateTimeFormat.ShortTimePattern = copy.DateTimeFormat.ShortTimePattern.Replace(" t", "t");

					s_ThemeCulture = copy;
				}

				return s_ThemeCulture;
			}
		}

		/// <summary>
		/// Gets the short time representation.
		/// E.g. for en-US: 3:46p
		/// </summary>
		public static string ShortTime { get { return GetShortTime(IcdEnvironment.GetLocalTime()); } }

		/// <summary>
		/// Gets the long data representation.
		/// E.g. for en-US: Tuesday, May 14, 2019
		/// </summary>
		public static string LongDate { get { return GetLongDate(IcdEnvironment.GetLocalTime()); } }

		#endregion

		#region Methods

		/// <summary>
		/// Gets the short time representation.
		/// E.g. for en-US: 3:46p
		/// </summary>
		public static string GetShortTime(DateTime dateTime)
		{
			return dateTime.ToString(ThemeCulture.DateTimeFormat.ShortTimePattern, ThemeCulture);
		}

		/// <summary>
		/// Gets the long data representation.
		/// E.g. for en-US: Tuesday, May 14, 2019
		/// </summary>
		public static string GetLongDate(DateTime dateTime)
		{
			return dateTime.ToString(ThemeCulture.DateTimeFormat.LongDatePattern, ThemeCulture);
		}

		#endregion
	}
}
