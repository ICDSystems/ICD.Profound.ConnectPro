using ICD.Connect.Panels.Mock;
using ICD.Profound.ConnectPRO.Tests.RoomTypes;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Helpers;
using NUnit.Framework;
using System;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Controls.Power;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Routing.Endpoints.Destinations;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Common
{
	public abstract class AbstractSettingsTest<TRoomType> : AbstractNavigationTest<TRoomType>
		where TRoomType : AbstractRoomType
	{
		#region Methods

		private void OpenSettings(TRoomType roomType)
		{
			// Simulate "open setting" button press
			NavigationHelpers.PressButton(115, roomType.Panel);
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(135, roomType.Panel));
		}

		private void CloseSettings(TRoomType roomType)
		{
			// Simulate "close setting" button press
			NavigationHelpers.PressButton(160, roomType.Panel);
			Assert.IsFalse(NavigationHelpers.CheckVisibilty(141, roomType.Panel));
		}

		private void EnterPasscode(TRoomType roomType)
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

		private void EnterNewPasscode(TRoomType roomType)
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

		private void SelectWeekdays(TRoomType roomType)
		{
			// Passcode Settings is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			// Simulate "weekdays" button press
			NavigationHelpers.PressButton(800, roomType.Panel);

			Assert.IsTrue(NavigationHelpers.CheckVisibilty(800, roomType.Panel));
		}

		private void SelectWeekends(TRoomType roomType)
		{
			// Passcode Settings is visible
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			// Simulate "weekdays" button press
			NavigationHelpers.PressButton(801, roomType.Panel);

			Assert.IsTrue(NavigationHelpers.CheckVisibilty(801, roomType.Panel));
		}

		private void EnableSystemPowerSettings(TRoomType roomType, bool weekdays, bool enable = true)
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

			Assert.AreEqual(enable,
			                weekdays
				                ? roomType.Room.WakeSchedule.WeekdayEnableWake && roomType.Room.WakeSchedule.WeekdayEnableSleep
				                : roomType.Room.WakeSchedule.WeekendEnableWake && roomType.Room.WakeSchedule.WeekendEnableSleep);
		}

		private void PowerSystemPowerSettings(TRoomType roomType, bool powerOn)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			foreach (IDestination destination in roomType.Room.Routing.Destinations.GetVideoDestinations())
			{
				if (roomType.Core.Originators.GetChild(destination.Device) is IDisplay display)
					display.PowerOn();
			}

			NavigationHelpers.PressButton((uint)(powerOn ? 812 : 813), roomType.Panel);

			// Check Power for displays (wake)
			if (!powerOn)
			{
				foreach (IDestination destination in roomType.Room.Routing.Destinations.GetVideoDestinations())
				{
					if (roomType.Core.Originators.GetChild(destination.Device) is IDisplay display)
						Assert.AreEqual(ePowerState.PowerOff, display.PowerState);
				}
			}

			// Check Power for the panels
			roomType.Room.Originators.GetInstancesRecursive<IPanelDevice>()
			        .SelectMany(panel => panel.Controls.GetControls<IPowerDeviceControl>())
			        .ForEach(c => Assert.AreEqual(powerOn, (c.PowerState == ePowerState.PowerOn)));

			CloseSettings(roomType);
		}

		private void IncrementPowerOnTime(TRoomType roomType, bool weekdays, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			TimeSpan currentWakeTime =
				(weekdays ? roomType.Room.WakeSchedule.WeekdayWakeTime : roomType.Room.WakeSchedule.WeekendWakeTime) ??
				new TimeSpan(7, 0, 0);
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
			var updatedWakeTime =
				(weekdays ? roomType.Room.WakeSchedule.WeekdayWakeTime : roomType.Room.WakeSchedule.WeekendWakeTime);

			Assert.AreEqual(expectedWakeTime, updatedWakeTime);
		}

		private void DecrementPowerOnTime(TRoomType roomType, bool weekdays, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			TimeSpan currentWakeTime =
				(weekdays ? roomType.Room.WakeSchedule.WeekdayWakeTime : roomType.Room.WakeSchedule.WeekendWakeTime) ??
				new TimeSpan(7, 0, 0);
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
			var updatedWakeTime =
				(weekdays ? roomType.Room.WakeSchedule.WeekdayWakeTime : roomType.Room.WakeSchedule.WeekendWakeTime);

			Assert.AreEqual(expectedWakeTime, updatedWakeTime);
		}

		private void IncrementPowerOffTime(TRoomType roomType, bool weekdays, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			TimeSpan currentSleepTime =
				(weekdays ? roomType.Room.WakeSchedule.WeekdaySleepTime : roomType.Room.WakeSchedule.WeekendSleepTime) ??
				new TimeSpan(19, 0, 0);
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
			var updatedSleepTime =
				(weekdays ? roomType.Room.WakeSchedule.WeekdaySleepTime : roomType.Room.WakeSchedule.WeekendSleepTime);

			Assert.AreEqual(expectedSleepTime, updatedSleepTime);
		}

		private void DecrementPowerOffTime(TRoomType roomType, bool weekdays, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));

			TimeSpan currentSleepTime =
				(weekdays ? roomType.Room.WakeSchedule.WeekdaySleepTime : roomType.Room.WakeSchedule.WeekendSleepTime) ??
				new TimeSpan(19, 0, 0);
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
			var updatedSleepTime =
				(weekdays ? roomType.Room.WakeSchedule.WeekdaySleepTime : roomType.Room.WakeSchedule.WeekendSleepTime);

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

		#endregion
	}
}
