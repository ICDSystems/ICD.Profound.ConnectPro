using ICD.Connect.Protocol.Sigs;
using ICD.Profound.ConnectPRO.Tests.RoomTypes;
using ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Helpers;
using NUnit.Framework;

namespace ICD.Profound.ConnectPRO.Tests.Themes.UserInterface.Navigation.Common
{
    public abstract class AbstractMeetingTest<TRoomType> : AbstractNavigationTest<TRoomType>
		where TRoomType : AbstractRoomType
    {
	    [Test]
	    public void StartMeetingTest()
	    {
		    using (TRoomType roomType = InstantiateRoomType())
		    {
			    // Simulate "start meeting" button press
			    NavigationHelpers.PressButton(91, roomType.Panel);

				Assert.IsTrue(roomType.Room.IsInMeeting);
		    }
	    }

	    [Test]
	    public void EndMeetingTest()
	    {
		    using (TRoomType roomType = InstantiateRoomType())
		    {
				// Simulate "start meeting" button press
			    NavigationHelpers.PressButton(91, roomType.Panel);

				Assert.IsTrue(roomType.Room.IsInMeeting);

				// Simulate "end meeting" button press (cancel)
			    NavigationHelpers.PressButton(30, roomType.Panel);
			    NavigationHelpers.PressButton(32, roomType.Panel);

				Assert.IsTrue(roomType.Room.IsInMeeting);

				// Simulate "end meeting" button press (yes)
			    NavigationHelpers.PressButton(30, roomType.Panel);
			    NavigationHelpers.PressButton(31, roomType.Panel);

				Assert.IsFalse(roomType.Room.IsInMeeting);
		    }
	    }
	}
}
