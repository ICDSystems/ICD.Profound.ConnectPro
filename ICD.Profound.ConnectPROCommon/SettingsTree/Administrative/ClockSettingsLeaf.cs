using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Settings.Localization;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.Administrative
{
	public sealed class ClockSettingsLeaf : AbstractSettingsLeaf
	{
		/// <summary>
		/// Returns true if the current culture uses a 24 hour time format.
		/// </summary>
		public bool Is24HourMode
		{
			get
			{
				if (Room == null)
					throw new InvalidOperationException("No room assigned to node");

				return
					Room.Core.Localization.CurrentCulture.Uses24HourFormat();
			}
		}

		/// <summary>
		/// Gets the current time of day.
		/// </summary>
		public DateTime ClockTime { get { return IcdEnvironment.GetLocalTime(); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ClockSettingsLeaf()
		{
			Name = "Clock";
			Icon = eSettingsIcon.Clock;
		}

		/// <summary>
		/// Sets the system clock time.
		/// </summary>
		/// <param name="dateTime"></param>
		public void SetClockTime(DateTime dateTime)
		{
			IcdEnvironment.SetLocalTime(dateTime);
		}

		/// <summary>
		/// Configures the culture as 24 using 24 hour time.
		/// </summary>
		/// <param name="hour24Mode"></param>
		public void Set24HourMode(bool hour24Mode)
		{
			if (Room == null)
				throw new InvalidOperationException("No room assigned to node");

			Localization.e24HourOverride mode =
				hour24Mode
					? Localization.e24HourOverride.Override24Hour
					: Localization.e24HourOverride.Override12Hour;

			Room.Core.Localization.Set24HourOverride(mode);
			SetDirty(true);
		}
	}
}
