using ICD.Connect.Panels.Mock;
using ICD.Profound.ConnectPRO.Tests.RoomTypes.Common;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Helpers;
using NUnit.Framework;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.PresentationSingle
{
    public abstract class AbstractSettingsTest<TRoomType> : AbstractNavigationTest<TRoomType>
		where TRoomType : AbstractRoomType
	{

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

		public void EnableWeekdaySystemPowerSettings(TRoomType roomType)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(800, roomType.Panel));

			NavigationHelpers.PressButton(810, roomType.Panel);

			CloseSettings(roomType);

			Assert.IsTrue(roomType.Room.WakeSchedule.WeekdayEnable);
			var wake = roomType.Room.WakeSchedule.WeekdayWakeTime;
			var sleep = roomType.Room.WakeSchedule.WeekendSleepTime;
			//var enable = roomType.Room.WakeSchedule.WeekdayEnable;
		}

		public void IncrementWeekdayPowerOnTime(TRoomType roomType, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(800, roomType.Panel));

			if (hour)
				NavigationHelpers.PressButton(802, roomType.Panel);
			else
			{
				NavigationHelpers.PressButton(804, roomType.Panel);
			}

			CloseSettings(roomType);

			var wake = roomType.Room.WakeSchedule.WeekdayWakeTime;
			var sleep = roomType.Room.WakeSchedule.WeekendSleepTime;

			Assert.IsFalse(roomType.Room.WakeSchedule.WeekdayEnable);
			//var enable = roomType.Room.WakeSchedule.WeekdayEnable;
		}
		public void DecrementWeekdayPowerOnTime(TRoomType roomType, bool hour = true)
		{
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(142, roomType.Panel));
			Assert.IsTrue(NavigationHelpers.CheckVisibilty(800, roomType.Panel));

			if(hour)
				NavigationHelpers.PressButton(803, roomType.Panel);
			else
			{
				NavigationHelpers.PressButton(805, roomType.Panel);
			}

			CloseSettings(roomType);

			var wake = roomType.Room.WakeSchedule.WeekdayWakeTime;
			var sleep = roomType.Room.WakeSchedule.WeekendSleepTime;

			Assert.IsFalse(roomType.Room.WakeSchedule.WeekdayEnable);
			//var enable = roomType.Room.WakeSchedule.WeekdayEnable;
		}

		//public void SelectWeekends(TRoomType roomType)
		//{
		//	// Passcode Settings is visible
		//	Assert.IsTrue(NavigationHelpers.CheckVisibilty(143, roomType.Panel));

		//	// Simulate "weekends" button press
		//	NavigationHelpers.PressButton(5, keypad, 800);

		//	NavigationHelpers.PressButton(5, keypad, 800);
		//	NavigationHelpers.PressButton(5, keypad, 800);
		//	NavigationHelpers.PressButton(5, keypad, 800);
		//	NavigationHelpers.PressButton(5, keypad, 800);
		//	NavigationHelpers.PressButton(12, keypad, 800);

		//	// Settings base is visible
		//	Assert.IsTrue(NavigationHelpers.CheckVisibilty(141, roomType.Panel));

		//	// Passcode Settings is visible
		//	Assert.IsTrue(NavigationHelpers.CheckVisibilty(143, roomType.Panel));
		//}

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
		public void SetSystemPowerPreferencesTest()
		{
			using (TRoomType roomType = InstantiateRoomType())
			{
				Assert.IsFalse(NavigationHelpers.CheckVisibilty(135, roomType.Panel));

				OpenSettings(roomType);
				EnterPasscode(roomType);
				SelectSystemPowerPreference(roomType);

				// Simulate "weekdays" button press
				SelectWeekdays(roomType);

				// Enable "weekdays" System Power Settings
				IncrementWeekdayPowerOnTime(roomType);
				DecrementWeekdayPowerOnTime(roomType);
				EnableWeekdaySystemPowerSettings(roomType);

				CloseSettings(roomType);
			}
		}
	}
}
