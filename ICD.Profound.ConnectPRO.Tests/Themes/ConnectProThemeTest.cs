using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Audio.Shure;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Settings.Core;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes;
using ICD.Profound.ConnectPRO.Themes.MicrophoneInterface;
using ICD.Profound.ConnectPRO.Themes.OsdInterface;
using ICD.Profound.ConnectPRO.Themes.UserInterface;
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
	    public void BuildUserInterfaces()
	    {
		    using (var core = new Core {Id = 100})
		    {
				//instantiating rooms and devices
				core.Originators.AddChild(new RoutingGraph {Id = 200});
			    var connectProTheme = new ConnectProTheme {Id = 300};
			    var connectProRoom1 = new ConnectProRoom {Id = 400};
			    var connectProRoom2 = new ConnectProRoom {Id = 500};
			    var mockPanelDevice = new MockPanelDevice {Id = 600};
			    var microphoneDevice = new ShureMxa910Device {Id = 700};
			    var osdDevice = new OsdPanelDevice {Id = 800};

				//Adding rooms and devices to core Originators
				core.Originators.AddChild(connectProRoom1);
			    core.Originators.AddChild(connectProRoom2);
			    core.Originators.AddChild(mockPanelDevice);
			    core.Originators.AddChild(connectProTheme);
			    core.Originators.AddChild(microphoneDevice);
			    core.Originators.AddChild(osdDevice);

				//Building User Interfaces for empty rooms
				connectProTheme.BuildUserInterfaces();

			    var uiFactories = connectProTheme.GetUiFactories().ToList();
			    var connectProUserInterfaceFactory = uiFactories[0];
			    var connectProMicrophoneInterfaceFactory = uiFactories[1];
			    var connectProOsdInterfaceFactory = uiFactories[2];

			    Assert.NotNull(connectProUserInterfaceFactory);
			    Assert.NotNull(connectProMicrophoneInterfaceFactory);
			    Assert.NotNull(connectProOsdInterfaceFactory);

			    var connectProUserInterfaceList = connectProUserInterfaceFactory.GetUserInterfaces().ToList();
			    var connectProMicrophoneInterfaceList =
				    connectProMicrophoneInterfaceFactory.GetUserInterfaces().ToList();
			    var connectProOsdInterfaceList = connectProOsdInterfaceFactory.GetUserInterfaces().ToList();

			    Assert.IsNull(connectProUserInterfaceList.FirstOrDefault());
			    Assert.IsNull(connectProOsdInterfaceList.FirstOrDefault());
			    Assert.IsNull(connectProMicrophoneInterfaceList.FirstOrDefault());

				//Adding devices to rooms (Panels and Microphone)
				connectProRoom1.Originators.Add(mockPanelDevice.Id, eCombineMode.Always);
			    connectProRoom2.Originators.Add(mockPanelDevice.Id, eCombineMode.Always);
			    connectProRoom2.Originators.Add(microphoneDevice.Id, eCombineMode.Always);

			    //Building User Interfaces for rooms
				connectProTheme.BuildUserInterfaces();

			    connectProUserInterfaceList = connectProUserInterfaceFactory.GetUserInterfaces().ToList();
			    connectProMicrophoneInterfaceList = connectProMicrophoneInterfaceFactory.GetUserInterfaces().ToList();
			    connectProOsdInterfaceList = connectProOsdInterfaceFactory.GetUserInterfaces().ToList();

			    Assert.AreEqual(mockPanelDevice, (connectProUserInterfaceList.FirstOrDefault() as ConnectProUserInterface)?.Panel);
			    Assert.IsNull(connectProOsdInterfaceList.FirstOrDefault());

			    Assert.AreEqual(connectProRoom1, (connectProUserInterfaceList.FirstOrDefault() as ConnectProUserInterface)?.Room);
			    Assert.AreEqual(connectProRoom2, (connectProMicrophoneInterfaceList.FirstOrDefault() as ConnectProMicrophoneInterface)?.Room);

				//Adding more devices to rooms (OSDs and Microphone)
				connectProRoom1.Originators.Add(osdDevice.Id, eCombineMode.Always);
			    connectProRoom1.Originators.Add(microphoneDevice.Id, eCombineMode.Always);
			    connectProRoom2.Originators.Add(osdDevice.Id, eCombineMode.Always);

			    //Building User Interfaces for rooms
				connectProTheme.BuildUserInterfaces();

			    connectProUserInterfaceList = connectProUserInterfaceFactory.GetUserInterfaces().ToList();
			    connectProMicrophoneInterfaceList = connectProMicrophoneInterfaceFactory.GetUserInterfaces().ToList();
			    connectProOsdInterfaceList = connectProOsdInterfaceFactory.GetUserInterfaces().ToList();

			    Assert.AreEqual(mockPanelDevice, (connectProUserInterfaceList.FirstOrDefault() as ConnectProUserInterface)?.Panel);
			    Assert.AreEqual(osdDevice, (connectProOsdInterfaceList.FirstOrDefault() as ConnectProOsdInterface)?.Panel);

			    Assert.AreEqual(connectProRoom1, (connectProUserInterfaceList.FirstOrDefault() as ConnectProUserInterface)?.Room);
			    Assert.AreEqual(connectProRoom1, (connectProOsdInterfaceList.FirstOrDefault() as ConnectProOsdInterface)?.Room);
			    Assert.AreEqual(connectProRoom1, (connectProMicrophoneInterfaceList.FirstOrDefault() as ConnectProMicrophoneInterface)?.Room);

			    Assert.IsNull(connectProUserInterfaceList.ElementAtOrDefault(1));
			    Assert.IsNull(connectProOsdInterfaceList.ElementAtOrDefault(1));
			    Assert.IsNull(connectProMicrophoneInterfaceList.ElementAtOrDefault(1));
		    }
	    }
    }
}
