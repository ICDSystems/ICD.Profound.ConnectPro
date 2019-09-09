using System;
using ICD.Common.Utils;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.SettingsTree.Administrative
{
	public sealed class ClockSettingsLeaf : AbstractSettingsLeaf
	{
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

			// Doesn't get saved back to settings, but improves UX ("oh look it saved my changes!")
			SetDirty(true);
		}
	}
}