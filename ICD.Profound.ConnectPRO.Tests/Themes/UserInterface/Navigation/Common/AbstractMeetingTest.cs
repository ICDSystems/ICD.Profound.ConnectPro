using ICD.Connect.Protocol.Sigs;
using ICD.Profound.ConnectPRO.Tests.RoomTypes;
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
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(91, 0, true));
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(91, 0, false));

				Assert.IsTrue(roomType.Room.IsInMeeting);
		    }
	    }

	    [Test]
	    public void EndMeetingTest()
	    {
		    using (TRoomType roomType = InstantiateRoomType())
		    {
			    // Simulate "start meeting" button press
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(91, 0, true));
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(91, 0, false));

				Assert.IsTrue(roomType.Room.IsInMeeting);

				// Simulate "end meeting" button press (cancel)
				roomType.Panel.RaiseOutputSigChange(new SigInfo(30, 0, true));
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(30, 0, false));
				roomType.Panel.RaiseOutputSigChange(new SigInfo(32, 0, true));
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(32, 0, false));

				Assert.IsTrue(roomType.Room.IsInMeeting);

			    // Simulate "end meeting" button press (yes)
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(30, 0, true));
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(30, 0, false));
				roomType.Panel.RaiseOutputSigChange(new SigInfo(31, 0, true));
			    roomType.Panel.RaiseOutputSigChange(new SigInfo(31, 0, false));

				Assert.IsFalse(roomType.Room.IsInMeeting);
		    }
	    }
	}
}
