using System;
using System.Globalization;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

namespace ICD.Profound.ConnectPRO.Themes
{
	public sealed class ConnectProDateFormatting
	{
		/// <summary>
		/// Raised when date formatting changes.
		/// </summary>
		public event EventHandler OnFormatChanged;

		private readonly ConnectProTheme m_Theme;

		private CultureInfo m_ThemeCulture;

		#region Properties

		/// <summary>
		/// Gets the custom culture for presenting data values to the end user in ConnectPro.
		/// </summary>
		public CultureInfo ThemeCulture
		{
			get
			{
				if (m_ThemeCulture == null)
				{
					CultureInfo copy = (CultureInfo)m_Theme.Core.Localization.CurrentCulture.Clone();

					// Custom AM/PM
					copy.DateTimeFormat = (DateTimeFormatInfo)copy.DateTimeFormat.Clone();
					if (copy.DateTimeFormat.AMDesignator == "AM" && copy.DateTimeFormat.PMDesignator == "PM")
					{
						copy.DateTimeFormat.AMDesignator = "a";
						copy.DateTimeFormat.PMDesignator = "p";

						// Janky, remove the space between minutes and AM/PM
						copy.DateTimeFormat.ShortTimePattern = copy.DateTimeFormat.ShortTimePattern.Replace(" t", "t");
					}

					m_ThemeCulture = copy;
				}

				return m_ThemeCulture;
			}
		}

		/// <summary>
		/// Gets the short time representation.
		/// E.g. for en-US: 3:46p
		/// </summary>
		public string ShortTime { get { return GetShortTime(IcdEnvironment.GetLocalTime()); } }

		/// <summary>
		/// Gets the long data representation.
		/// E.g. for en-US: Tuesday, May 14, 2019
		/// </summary>
		public string LongDate { get { return GetLongDate(IcdEnvironment.GetLocalTime()); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		public ConnectProDateFormatting(ConnectProTheme theme)
		{
			if (theme == null)
				throw new ArgumentNullException("theme");

			m_Theme = theme;

			m_Theme.Core.Localization.OnCultureChanged += LocalizationOnCultureChanged;
		}

		#region Methods

		/// <summary>
		/// Gets the short time representation.
		/// E.g. for en-US: 3:46p
		/// </summary>
		public string GetShortTime(DateTime dateTime)
		{
			return dateTime.ToString(ThemeCulture.DateTimeFormat.ShortTimePattern, ThemeCulture);
		}

		/// <summary>
		/// Gets the long data representation.
		/// E.g. for en-US: Tuesday, May 14, 2019
		/// </summary>
		public string GetLongDate(DateTime dateTime)
		{
			return dateTime.ToString(ThemeCulture.DateTimeFormat.LongDatePattern, ThemeCulture);
		}

		#endregion

		/// <summary>
		/// Called when the core localization changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void LocalizationOnCultureChanged(object sender, EventArgs eventArgs)
		{
			m_ThemeCulture = null;

			OnFormatChanged.Raise(this);
		}
	}
}
