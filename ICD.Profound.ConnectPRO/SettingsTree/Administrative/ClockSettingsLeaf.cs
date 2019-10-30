using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Settings.Localization;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Administrative
{
	public sealed class ClockSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Returns true if the current culture uses a 24 hour time format.
		/// </summary>
		public bool Is24HourMode { get { return Room.Core.Localization.CurrentCulture.Uses24HourFormat(); } }

		/// <summary>
		/// Gets the current time of day.
		/// </summary>
		public TimeSpan ClockTime { get { return IcdEnvironment.GetLocalTime().TimeOfDay; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ClockSettingsLeaf(IConnectProRoom room)
			: base(room)
		{
			Name = "Clock";
			Icon = SettingsTreeIcons.ICON_CLOCK;
		}

		/// <summary>
		/// Sets the system clock time.
		/// </summary>
		/// <param name="time"></param>
		public void SetClockTime(TimeSpan time)
		{
			// Keep precision
			DateTime now = IcdEnvironment.GetLocalTime();
			DateTime dateTime = new DateTime(now.Year, now.Month, now.Day,
			                                 time.Hours, time.Minutes, time.Seconds,
			                                 now.Millisecond);

			IcdEnvironment.SetLocalTime(dateTime);
		}

		/// <summary>
		/// Configures the culture as 24 using 24 hour time.
		/// </summary>
		/// <param name="hour24Mode"></param>
		public void Set24HourMode(bool hour24Mode)
		{
			Localization.e24HourOverride mode =
				hour24Mode
					? Localization.e24HourOverride.Override24Hour
					: Localization.e24HourOverride.Override12Hour;

			Room.Core.Localization.Set24HourOverride(mode);

			SetDirty(true);
		}
	}
}
