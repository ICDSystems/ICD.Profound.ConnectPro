using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Xml;
using ICD.Connect.Audio.VolumePoints;
using ICD.Connect.Displays.Mock.Devices;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Mock.Source;
using ICD.Connect.Routing.Mock.Switcher;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Settings.Core;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes;

namespace ICD.Profound.ConnectPRO.Tests.RoomTypes
{
	public sealed class PresentationSingleRoomType : AbstractRoomType
	{
		private readonly ConnectProRoom m_room;
		private readonly MockPanelDevice m_panel;
		private readonly ICore m_core;

		public override ConnectProRoom Room { get { return m_room; } }
		public override MockPanelDevice Panel { get { return m_panel; } }
		public override ICore Core { get { return m_core; } }

		public PresentationSingleRoomType()
		{
			m_core = new Core {Id = 100};

			//instantiating RoutingGraph and adding to core
			var routingGraph = new RoutingGraph {Id = 200};
			m_core.Originators.AddChild(routingGraph);

			//instantiating rooms and devices
			var connectProTheme = new ConnectProTheme {Id = 100};
			m_room = new ConnectProRoom {Id = 80000266, Passcode = "0000"};
			m_panel = new MockPanelDevice {Id = 20001602};
			var osdPanel = new OsdPanelDevice {Id = 20001605};
			var atUhdHdvs300 = new MockSwitcherDevice {Id = 20001600};
			var mockSourceDevice1 = new MockSourceDevice {Id = 20001604};
			var mockSourceDevice2 = new MockSourceDevice {Id = 20001606};
			var sharpDisplay = new MockDisplayWithAudio {Id = 20001661};
			var connectProVolumePoint = new VolumePoint {Id = 90000045, DeviceId = 20001661 };
			connectProTheme.WebConferencingInstructions.Parse(Properties.Resources.WebConferencing);

			//Adding rooms and devices to core Originators
			m_core.Originators.AddChild(m_room);
			m_core.Originators.AddChild(m_panel);
			m_core.Originators.AddChild(connectProTheme);
			m_core.Originators.AddChild(osdPanel);
			m_core.Originators.AddChild(atUhdHdvs300);
			m_core.Originators.AddChild(mockSourceDevice1);
			m_core.Originators.AddChild(mockSourceDevice2);
			m_core.Originators.AddChild(sharpDisplay);
			m_core.Originators.AddChild(connectProVolumePoint);

			//Adding devices to room
			m_room.Originators.Add(m_panel.Id, eCombineMode.Always);
			m_room.Originators.Add(osdPanel.Id, eCombineMode.Always);
			m_room.Originators.Add(atUhdHdvs300.Id, eCombineMode.Always);
			m_room.Originators.Add(mockSourceDevice1.Id, eCombineMode.Always);
			m_room.Originators.Add(mockSourceDevice2.Id, eCombineMode.Always);
			m_room.Originators.Add(sharpDisplay.Id, eCombineMode.Always);
			m_room.Originators.Add(connectProVolumePoint.Id, eCombineMode.Always);

			// Add sources/destinations to the routing graph and core
			var display = new Destination(60000040, 20001661, 0, new[] {1, 2, 3}, "Display", false, 0, false) {ConnectionType = eConnectionType.Video | eConnectionType.Audio};
			routingGraph.Destinations.AddChild(display);
			m_core.Originators.AddChild(display);
			m_room.Originators.Add(display.Id, eCombineMode.Always);

			var laptop = new ConnectProSource
			{
				Name = "Laptop",
				Order = 0,
				Id = 50000046,
				Device = 20001606,
				ConnectionType = eConnectionType.Video | eConnectionType.Audio
			};
			laptop.SetAddresses(new[] { 1, 2, 3 });
			routingGraph.Sources.AddChild(laptop);
			m_core.Originators.AddChild(laptop);
			m_room.Originators.Add(laptop.Id, eCombineMode.Always);

			var webConferencing = new ConnectProSource
			{
				Name = "Web Conferencing",
				Order = 1,
				Id = 50000047,
				Device = 20001604,
				ConnectionType = eConnectionType.Video | eConnectionType.Audio,
				ControlOverride = eControlOverride.WebConference
			};
			webConferencing.SetAddresses(new[] { 1, 2});
			routingGraph.Sources.AddChild(webConferencing);
			m_core.Originators.AddChild(webConferencing);
			m_room.Originators.Add(webConferencing.Id, eCombineMode.Always);

			// Add connections to the routing graph and core
			var connections = new List<Connection>
			{
				new Connection(30000757, 20001605, 0, 1, 20001600, 0, 5, eConnectionType.Audio | eConnectionType.Video),
				new Connection(30000758, 20001600, 0, 1, 20001661, 0, 1, eConnectionType.Audio | eConnectionType.Video),
				new Connection(30000762, 20001606, 0, 1, 20001600, 0, 1, eConnectionType.Audio | eConnectionType.Video),
				new Connection(30000763, 20001604, 0, 1, 20001600, 0, 2, eConnectionType.Audio | eConnectionType.Video)
			};
			routingGraph.Connections.AddChildren(connections);
			m_core.Originators.AddChildren(connections);

			//Building User Interfaces for room
			connectProTheme.BuildUserInterfaces();
		}

		public override void Dispose()
		{
			ServiceProvider.RemoveService(m_core);
			var core = m_core as Core;
			core?.Dispose();
		}
	}
}
