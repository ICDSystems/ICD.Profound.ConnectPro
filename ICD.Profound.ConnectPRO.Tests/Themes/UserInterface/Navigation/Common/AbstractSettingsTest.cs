using ICD.Connect.Panels.Mock;
using ICD.Profound.ConnectPRO.Tests.RoomTypes;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Helpers;
using NUnit.Framework;
using System;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Routing.Endpoints.Destinations;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Common
{
    public abstract class AbstractSettingsTest<TRoomType> : AbstractNavigationTest<TRoomType>
		where TRoomType : AbstractRoomType
	{

		#region Methods

		public void OpenSettings(TRoomType roomType)
	    {
		    // Simulate "open setting" button press
		    NavigationHelpers.PressButton(115, roomType.Panel);
		    Assert.IsTrue(NavigationHelpers.CheckVisibilty(135, roomType.Panel));
		}

		public void CloseSettings(TRoomType roomType)
		{
			// Simulate "close setting" button press
			NavigationHelpers.PressButton(160, roomType.Panel);
			Assert.IsFalse(NavigationHelpers.CheckVisibilty(141, roomType.Panel));
		}

		public void EnterPasscode(TRoomType roomType)
	    {
		    // Simulate "enter passcode" button press
		    var keypad = roomType.Panel.SmartObjects[651] as MockSmartObject;

		    Assert.IsNotNull(keypad);

		    NavigationHelpers.PressButton(1, keypad, 651);
		    NavigationHelpers.PressButton(1, keypad, 651);
		    NavigationHelpers.PressButton(1, keypad, 651);
		    NavigationHelpers.PressButton(1, keypad, 651);
		    NavigationHelpers.PressButton(12, keypad, 651);

		    Assert.IsTrue(NavigationHelpers.CheckVisibilty(135, roomType.Panel));
		    Assert.IsFalse(NavigationHelpers.CheckVisibilty(141, roomType.Panel));
		    Assert.IsFalse(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			NavigationHelpers.PressButton(10, keypad, 651);
		    NavigationHelpers.PressButton(10, keypad, 651);
		    NavigationHelpers.PressButton(10, keypad, 651);
		    NavigationHelpers.PressButton(10, keypad, 651);
		    NavigationHelpers.PressButton(12, keypad, 651);

			// Settings base is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(141, roomType.Panel));

			// System power preferences is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));
		}

		public void EnterNewPasscode(TRoomType roomType)
		{
			// Passcode Settings is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(143, roomType.Panel));

			// Simulate "enter passcode" button press
			var keypad = roomType.Panel.SmartObjects[800] as MockSmartObject;

			Assert.IsNotNull(keypad);

			NavigationHelpers.PressButton(5, keypad, 800);
			NavigationHelpers.PressButton(5, keypad, 800);
			NavigationHelpers.PressButton(5, keypad, 800);
			NavigationHelpers.PressButton(5, keypad, 800);
			NavigationHelpers.PressButton(12, keypad, 800);

			// Settings base is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(141, roomType.Panel));

			// Passcode Settings is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(143, roomType.Panel));
		}

		public void SelectPasscodeSettings(TRoomType roomType)
		{
			NavigationHelpers.PressListButton(1, 652, 2, roomType.Panel);

			Assert.IsFalse(NavigationHelpers.CheckVisibilty(142, roomType.Panel));
			Assert.IsFalse(NavigationHelpers.CheckVisibilty(144, roomType.Panel));
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(143, roomType.Panel));
		}

		public void SelectDirectory(TRoomType roomType)
		{
			NavigationHelpers.PressListButton(1, 652, 3, roomType.Panel);

			Assert.IsFalse(NavigationHelpers.CheckVisibilty(142, roomType.Panel));
			Assert.IsFalse(NavigationHelpers.CheckVisibilty(143, roomType.Panel));
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(144, roomType.Panel));
		}

		public void SelectSystemPowerPreference(TRoomType roomType)
		{
			NavigationHelpers.PressListButton(1, 652, 1, roomType.Panel);

			Assert.IsFalse(NavigationHelpers.CheckVisibilty(143, roomType.Panel));
			Assert.IsFalse(NavigationHelpers.CheckVisibilty(144, roomType.Panel));
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));
		}

		public void SelectWeekdays(TRoomType roomType)
		{
			// Passcode Settings is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			// Simulate "weekdays" button press
			NavigationHelpers.PressButton(800, roomType.Panel);

			Assert.IsTrue(NavigationHelpers.CheckVisibilty(800, roomType.Panel));
		}

		public void SelectWeekends(TRoomType roomType)
		{
			// Passcode Settings is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			// Simulate "weekdays" button press
			NavigationHelpers.PressButton(801, roomType.Panel);

			Assert.IsTrue(NavigationHelpers.CheckVisibilty(801, roomType.Panel));
		}

		public void EnableSystemPowerSettings(TRoomType roomType, bool weekdays, bool enable = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			if (enable)
			{
				NavigationHelpers.PressButton(810, roomType.Panel);
			}
			else
			{
				NavigationHelpers.PressButton(811, roomType.Panel);
			}

			CloseSettings(roomType);

			Assert.AreEqual(enable, weekdays ? roomType.Room.WakeSchedule.WeekdayEnable : roomType.Room.WakeSchedule.WeekendEnable);
		}

		public void PowerSystemPowerSettings(TRoomType roomType, bool powerOn)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			foreach (IDestination destination in roomType.Room.Routing.GetDisplayDestinations())
			{
				if (roomType.Core.Originators.GetChild(destination.Device) is IDisplay display)
					display.PowerOn();
			}

			NavigationHelpers.PressButton((uint) (powerOn ? 812 : 813), roomType.Panel);

			// Check Power for displays (wake)
			if (!powerOn)
			{
				foreach (IDestination destination in roomType.Room.Routing.GetDisplayDestinations())
				{
					if (roomType.Core.Originators.GetChild(destination.Device) is IDisplay display)
						Assert.AreEqual(false, display.IsPowered);
				}
			}

			// Check Power for the panels
			roomType.Room.Originators.GetInstancesRecursive<IPanelDevice>()
				.SelectMany(panel => panel.Controls.GetControls<IPowerDeviceControl>())
				.ForEach(c => Assert.AreEqual(powerOn, c.IsPowered));

			CloseSettings(roomType);
		}

		public void IncrementPowerOnTime(TRoomType roomType, bool weekdays, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			TimeSpan currentWakeTime = (weekdays ? roomType.Room.WakeSchedule.WeekdayWakeTime : roomType.Room.WakeSchedule.WeekendWakeTime) ?? new TimeSpan(7, 0, 0);
			TimeSpan? expectedWakeTime = null;

			if (hour)
			{
				NavigationHelpers.PressButton(802, roomType.Panel);
				expectedWakeTime = currentWakeTime.Add(TimeSpan.FromHours(1));
			}
			else
			{
				NavigationHelpers.PressButton(804, roomType.Panel);
				expectedWakeTime = currentWakeTime.Add(TimeSpan.FromMinutes(1));
			}

			CloseSettings(roomType);
			var updatedWakeTime = (weekdays ? roomType.Room.WakeSchedule.WeekdayWakeTime : roomType.Room.WakeSchedule.WeekendWakeTime);

			Assert.AreEqual(expectedWakeTime, updatedWakeTime);
		}

		public void DecrementPowerOnTime(TRoomType roomType, bool weekdays, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			TimeSpan currentWakeTime = (weekdays ? roomType.Room.WakeSchedule.WeekdayWakeTime : roomType.Room.WakeSchedule.WeekendWakeTime) ?? new TimeSpan(7, 0, 0);
			TimeSpan? expectedWakeTime = null;

			if (hour)
			{
				NavigationHelpers.PressButton(803, roomType.Panel);
				expectedWakeTime = currentWakeTime.Add(TimeSpan.FromHours(-1));
			}
			else
			{
				NavigationHelpers.PressButton(805, roomType.Panel);
				expectedWakeTime = currentWakeTime.Add(TimeSpan.FromMinutes(-1));
			}

			CloseSettings(roomType);
			var updatedWakeTime = (weekdays ? roomType.Room.WakeSchedule.WeekdayWakeTime : roomType.Room.WakeSchedule.WeekendWakeTime);

			Assert.AreEqual(expectedWakeTime, updatedWakeTime);
		}

		public void IncrementPowerOffTime(TRoomType roomType, bool weekdays, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			TimeSpan currentSleepTime = (weekdays ? roomType.Room.WakeSchedule.WeekdaySleepTime : roomType.Room.WakeSchedule.WeekendSleepTime) ?? new TimeSpan(19, 0, 0);
			TimeSpan? expectedSleepTime = null;

			if (hour)
			{
				NavigationHelpers.PressButton(806, roomType.Panel);
				expectedSleepTime = currentSleepTime.Add(TimeSpan.FromHours(1));
			}
			else
			{
				NavigationHelpers.PressButton(808, roomType.Panel);
				expectedSleepTime = currentSleepTime.Add(TimeSpan.FromMinutes(1));
			}

			CloseSettings(roomType);
			var updatedSleepTime = (weekdays ? roomType.Room.WakeSchedule.WeekdaySleepTime : roomType.Room.WakeSchedule.WeekendSleepTime);

			Assert.AreEqual(expectedSleepTime, updatedSleepTime);
		}

		public void DecrementPowerOffTime(TRoomType roomType, bool weekdays, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			TimeSpan currentSleepTime = (weekdays ? roomType.Room.WakeSchedule.WeekdaySleepTime : roomType.Room.WakeSchedule.WeekendSleepTime) ?? new TimeSpan(19, 0, 0);
			TimeSpan? expectedSleepTime = null;

			if (hour)
			{
				NavigationHelpers.PressButton(807, roomType.Panel);
				expectedSleepTime = currentSleepTime.Add(TimeSpan.FromHours(-1));
			}
			else
			{
				NavigationHelpers.PressButton(809, roomType.Panel);
				expectedSleepTime = currentSleepTime.Add(TimeSpan.FromMinutes(-1));
			}

			CloseSettings(roomType);
			var updatedSleepTime = (weekdays ? roomType.Room.WakeSchedule.WeekdaySleepTime : roomType.Room.WakeSchedule.WeekendSleepTime);

			Assert.AreEqual(expectedSleepTime, updatedSleepTime);
		}

		#endregion

		#region Tests

		[Test]
	    public void CurrentPasscodeTest()
	    {
		    using (TRoomType roomType = InstantiateRoomType())
		    {
			    Assert.IsFalse(NavigationHelpers.CheckVisibilty(135, roomType.Panel));

				OpenSettings(roomType);
				EnterPasscode(roomType);
			}
		}

	    [Test]
	    public void ChangePasscodeTest()
	    {
		    using (TRoomType roomType = InstantiateRoomType())
		    {
			    Assert.IsFalse(NavigationHelpers.CheckVisibilty(135, roomType.Panel));

				OpenSettings(roomType);
			    EnterPasscode(roomType);
			    SelectPasscodeSettings(roomType);
				EnterNewPasscode(roomType);
			    EnterNewPasscode(roomType);
				Assert.IsTrue(roomType.Room.Passcode == "5555");
			}
	    }

		[Test]
		public void SystemPowerPreferencesTests()
		{
			using (TRoomType roomType = InstantiateRoomType())
			{
				Assert.IsFalse(NavigationHelpers.CheckVisibilty(135, roomType.Panel));

				void SystemPowerPreferenceNavigation()
				{
					OpenSettings(roomType);
					EnterPasscode(roomType);
					SelectSystemPowerPreference(roomType);
				}

				void SystemPowerPreferenceButtonTests(bool weekdays)
				{
					SystemPowerPreferenceNavigation();

					if (weekdays)
					{
						// Simulate "weekdays" button press
						SelectWeekdays(roomType);
					}
					else
					{
						// Simulate "weekends" button press
						SelectWeekends(roomType);
					}

					// Increment wake time (hour)
					IncrementPowerOnTime(roomType, weekdays);

					// Decrement wake time (hour)
					SystemPowerPreferenceNavigation();
					DecrementPowerOnTime(roomType, weekdays);

					// Increment sleep time (hour)
					SystemPowerPreferenceNavigation();
					IncrementPowerOffTime(roomType, weekdays);

					// Decrement sleep time (hour)
					SystemPowerPreferenceNavigation();
					DecrementPowerOffTime(roomType, weekdays);

					// Enable System Power Settings
					SystemPowerPreferenceNavigation();
					EnableSystemPowerSettings(roomType, weekdays);

					// Increment wake time (minute)
					SystemPowerPreferenceNavigation();
					IncrementPowerOnTime(roomType, weekdays, false);

					// Decrement wake time (minute)
					SystemPowerPreferenceNavigation();
					DecrementPowerOnTime(roomType, weekdays, false);

					// Increment sleep time (minute)
					SystemPowerPreferenceNavigation();
					IncrementPowerOffTime(roomType, weekdays, false);

					// Decrement sleep time (minute)
					SystemPowerPreferenceNavigation();
					DecrementPowerOffTime(roomType, weekdays, false);

					// Disable System Power Settings
					SystemPowerPreferenceNavigation();
					EnableSystemPowerSettings(roomType, weekdays, false);

					// Power On System Power Settings
					SystemPowerPreferenceNavigation();
					PowerSystemPowerSettings(roomType, true);

					// Power Off System Power Settings
					SystemPowerPreferenceNavigation();
					PowerSystemPowerSettings(roomType, false);
				}

				// Test Weekdays
				SystemPowerPreferenceButtonTests(true);

				// Test Weekends
				SystemPowerPreferenceButtonTests(false);
			}
		}

		#endregion

	}
}
