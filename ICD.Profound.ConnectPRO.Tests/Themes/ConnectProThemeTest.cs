using System.Linq;
using ICD.Connect.Partitioning;
using ICD.Profound.ConnectPRO.Tests.RoomTypes;
using ICD.Profound.ConnectPRO.Themes.UserInterface;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface;
using ICD.Profound.ConnectPROCommon.Themes.ShureMicrophoneInterface;
using NUnit.Framework;

namespace ICD.Profound.ConnectPRO.Tests.Themes
{
	[TestFixture]
    public sealed class ConnectProThemeTest
    {

	    /// <summary>
	    /// Clears and rebuilds the user interfaces.
	    /// </summary>
	    [Test]
	    public void BuildUserInterfacesEmptyRoom()
	    {
		    using (var simpleTestRoomType = new SimpleTestRoomType())
		    {
			    var connectProUserInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(0);
			    var connectProMicrophoneInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(1);
			    var connectProOsdInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(2);

			    Assert.NotNull(connectProUserInterfaceFactory);
			    Assert.NotNull(connectProMicrophoneInterfaceFactory);
			    Assert.NotNull(connectProOsdInterfaceFactory);

			    var connectProUserInterfaceList = connectProUserInterfaceFactory.GetUserInterfaces().ToList();
			    var connectProMicrophoneInterfaceList = connectProMicrophoneInterfaceFactory.GetUserInterfaces().ToList();
			    var connectProOsdInterfaceList = connectProOsdInterfaceFactory.GetUserInterfaces().ToList();

			    Assert.IsNull(connectProUserInterfaceList.FirstOrDefault());
			    Assert.IsNull(connectProOsdInterfaceList.FirstOrDefault());
			    Assert.IsNull(connectProMicrophoneInterfaceList.FirstOrDefault());
			}
	    }

	    /// <summary>
	    /// Clears and rebuilds the user interfaces.
	    /// </summary>
	    [Test]
	    public void BuildUserInterfacesPanelMicrophone()
	    {
		    using (var simpleTestRoomType = new SimpleTestRoomType())
		    {
				//Adding devices to rooms (Panels and Microphone)
			    simpleTestRoomType.ConnectProRoom1.Originators.Add(simpleTestRoomType.MockPanelDevice.Id, eCombineMode.Always);
			    simpleTestRoomType.ConnectProRoom1.Originators.Add(simpleTestRoomType.MockPanelDevice.Id, eCombineMode.Always);
			    simpleTestRoomType.ConnectProRoom2.Originators.Add(simpleTestRoomType.MicrophoneDevice.Id, eCombineMode.Always);

				//Building User Interfaces for rooms
			    simpleTestRoomType.ConnectProTheme.BuildUserInterfaces();

			    var connectProUserInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(0);
			    var connectProMicrophoneInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(1);
			    var connectProOsdInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(2);

			    Assert.NotNull(connectProUserInterfaceFactory);
			    Assert.NotNull(connectProMicrophoneInterfaceFactory);
			    Assert.NotNull(connectProOsdInterfaceFactory);

				var connectProUserInterfaceList = connectProUserInterfaceFactory.GetUserInterfaces().ToList();
			    var connectProMicrophoneInterfaceList = connectProMicrophoneInterfaceFactory.GetUserInterfaces().ToList();
			    var connectProOsdInterfaceList = connectProOsdInterfaceFactory.GetUserInterfaces().ToList();

			    Assert.AreEqual(simpleTestRoomType.MockPanelDevice, (connectProUserInterfaceList.FirstOrDefault() as ConnectProUserInterface)?.Panel);
			    Assert.IsNull(connectProOsdInterfaceList.FirstOrDefault());

			    Assert.AreEqual(simpleTestRoomType.ConnectProRoom1, (connectProUserInterfaceList.FirstOrDefault() as ConnectProUserInterface)?.Room);
			    Assert.AreEqual(simpleTestRoomType.ConnectProRoom2, (connectProMicrophoneInterfaceList.FirstOrDefault() as ConnectProShureMicrophoneInterface)?.Room);
			}
	    }

	    /// <summary>
	    /// Clears and rebuilds the user interfaces.
	    /// </summary>
	    [Test]
	    public void BuildUserInterfacesPanelMicrophoneOsd()
	    {
		    using (var simpleTestRoomType = new SimpleTestRoomType())
		    {
			    //Adding devices to rooms (Panels, Microphones and OSDs)
			    simpleTestRoomType.ConnectProRoom1.Originators.Add(simpleTestRoomType.MockPanelDevice.Id, eCombineMode.Always);
			    simpleTestRoomType.ConnectProRoom1.Originators.Add(simpleTestRoomType.MockPanelDevice.Id, eCombineMode.Always);
			    simpleTestRoomType.ConnectProRoom2.Originators.Add(simpleTestRoomType.MicrophoneDevice.Id, eCombineMode.Always);
			    simpleTestRoomType.ConnectProRoom1.Originators.Add(simpleTestRoomType.OsdDevice.Id, eCombineMode.Always);
			    simpleTestRoomType.ConnectProRoom1.Originators.Add(simpleTestRoomType.MicrophoneDevice.Id, eCombineMode.Always);
				simpleTestRoomType.ConnectProRoom2.Originators.Add(simpleTestRoomType.OsdDevice.Id, eCombineMode.Always);

				//Building User Interfaces for rooms
			    simpleTestRoomType.ConnectProTheme.BuildUserInterfaces();

			    var connectProUserInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(0);
			    var connectProMicrophoneInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(1);
			    var connectProOsdInterfaceFactory = simpleTestRoomType.GetInterfaces().ElementAt(2);

			    Assert.NotNull(connectProUserInterfaceFactory);
			    Assert.NotNull(connectProMicrophoneInterfaceFactory);
			    Assert.NotNull(connectProOsdInterfaceFactory);

			    var connectProUserInterfaceList = connectProUserInterfaceFactory.GetUserInterfaces().ToList();
			    var connectProMicrophoneInterfaceList = connectProMicrophoneInterfaceFactory.GetUserInterfaces().ToList();
			    var connectProOsdInterfaceList = connectProOsdInterfaceFactory.GetUserInterfaces().ToList();

			    Assert.AreEqual(simpleTestRoomType.MockPanelDevice, (connectProUserInterfaceList.FirstOrDefault() as ConnectProUserInterface)?.Panel);

			    Assert.AreEqual(simpleTestRoomType.ConnectProRoom1, (connectProUserInterfaceList.FirstOrDefault() as ConnectProUserInterface)?.Room);
			    Assert.AreEqual(simpleTestRoomType.ConnectProRoom1, (connectProMicrophoneInterfaceList.FirstOrDefault() as ConnectProShureMicrophoneInterface)?.Room);
			    Assert.AreEqual(simpleTestRoomType.ConnectProRoom1, (connectProOsdInterfaceList.FirstOrDefault() as ConnectProOsdInterface)?.Room);
			}
	    }
	}
}
