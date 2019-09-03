using System.Collections.Generic;
using ICD.Common.Utils.Services;
using ICD.Connect.Audio.VolumePoints;
using ICD.Connect.Displays.Mock.Devices;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning;
using ICD.Connect.Partitioning.PartitionManagers;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Mock.Source;
using ICD.Connect.Routing.Mock.Switcher;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Settings.Cores;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Rooms.Single;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes;

namespace ICD.Profound.ConnectPRO.Tests.RoomTypes
{
	public sealed class PresentationSingleRoomType : AbstractRoomType
	{
		private readonly IConnectProRoom m_Room;
		private readonly MockPanelDevice m_Panel;
		private readonly ICore m_Core;

		public override IConnectProRoom Room { get { return m_Room; } }
		public override MockPanelDevice Panel { get { return m_Panel; } }
		public override ICore Core { get { return m_Core; } }

		public PresentationSingleRoomType()
		{
			m_Core = new Core {Id = 100};

			// Instantiating RoutingGraph and adding to core
			var routingGraph = new RoutingGraph {Id = 200};
			m_Core.Originators.AddChild(routingGraph);

			// Instantiating PartitionManager and adding to core
			var partitionManager = new PartitionManager {Id = 201};
			m_Core.Originators.AddChild(partitionManager);

			// Instantiating theme
			var connectProTheme = new ConnectProTheme {Id = 202};
			m_Core.Originators.AddChild(connectProTheme);
			connectProTheme.WebConferencingInstructions.Parse(Properties.Resources.WebConferencing);

			// Instantiating rooms and devices
			m_Room = new ConnectProRoom {Id = 80000266, Passcode = "0000"};
			m_Panel = new MockPanelDevice {Id = 20001602};
			var osdPanel = new OsdPanelDevice {Id = 20001605};
			var atUhdHdvs300 = new MockSwitcherDevice {Id = 20001600};
			var mockSourceDevice1 = new MockSourceDevice {Id = 20001604};
			var mockSourceDevice2 = new MockSourceDevice {Id = 20001606};
			var sharpDisplay = new MockDisplayWithAudio {Id = 20001661};
			var connectProVolumePoint = new VolumePoint {Id = 90000045, DeviceId = sharpDisplay.Id};

			// Adding rooms and devices to core Originators
			m_Core.Originators.AddChild(m_Room);
			m_Core.Originators.AddChild(m_Panel);
			m_Core.Originators.AddChild(osdPanel);
			m_Core.Originators.AddChild(atUhdHdvs300);
			m_Core.Originators.AddChild(mockSourceDevice1);
			m_Core.Originators.AddChild(mockSourceDevice2);
			m_Core.Originators.AddChild(sharpDisplay);
			m_Core.Originators.AddChild(connectProVolumePoint);

			// Adding devices to room
			m_Room.Originators.Add(m_Panel.Id, eCombineMode.Always);
			m_Room.Originators.Add(osdPanel.Id, eCombineMode.Always);
			m_Room.Originators.Add(atUhdHdvs300.Id, eCombineMode.Always);
			m_Room.Originators.Add(mockSourceDevice1.Id, eCombineMode.Always);
			m_Room.Originators.Add(mockSourceDevice2.Id, eCombineMode.Always);
			m_Room.Originators.Add(sharpDisplay.Id, eCombineMode.Always);
			m_Room.Originators.Add(connectProVolumePoint.Id, eCombineMode.Always);

			// Add sources/destinations to the routing graph and core
			var display =
				new Destination(60000040, sharpDisplay.Id, 0, new[] {1, 2, 3}, "Display", false, 0, false)
				{
					ConnectionType = eConnectionType.Video | eConnectionType.Audio
				};
			routingGraph.Destinations.AddChild(display);
			m_Core.Originators.AddChild(display);
			m_Room.Originators.Add(display.Id, eCombineMode.Always);

			var laptop = new ConnectProSource
			{
				Name = "Laptop",
				Order = 0,
				Id = 50000046,
				Device = mockSourceDevice2.Id,
				ConnectionType = eConnectionType.Video | eConnectionType.Audio
			};
			laptop.SetAddresses(new[] {1, 2, 3});
			routingGraph.Sources.AddChild(laptop);
			m_Core.Originators.AddChild(laptop);
			m_Room.Originators.Add(laptop.Id, eCombineMode.Always);

			var webConferencing = new ConnectProSource
			{
				Name = "Web Conferencing",
				Order = 1,
				Id = 50000047,
				Device = mockSourceDevice1.Id,
				ConnectionType = eConnectionType.Video | eConnectionType.Audio,
				ControlOverride = eControlOverride.WebConference
			};
			webConferencing.SetAddresses(new[] {1, 2});
			routingGraph.Sources.AddChild(webConferencing);
			m_Core.Originators.AddChild(webConferencing);
			m_Room.Originators.Add(webConferencing.Id, eCombineMode.Always);

			// Add connections to the routing graph and core
			var connections = new List<Connection>
			{
				// Sources to switcher
				new Connection(30000762, mockSourceDevice1.Id, 0, 1, atUhdHdvs300.Id, 0, 1,
				               eConnectionType.Audio | eConnectionType.Video),
				new Connection(30000763, mockSourceDevice2.Id, 0, 1, atUhdHdvs300.Id, 0, 2,
				               eConnectionType.Audio | eConnectionType.Video),
				new Connection(30000757, osdPanel.Id, 0, 1, atUhdHdvs300.Id, 0, 5, eConnectionType.Audio | eConnectionType.Video),

				// Switcher to display
				new Connection(30000758, atUhdHdvs300.Id, 0, 1, sharpDisplay.Id, 0, 1,
				               eConnectionType.Audio | eConnectionType.Video)
			};
			routingGraph.Connections.AddChildren(connections);
			m_Core.Originators.AddChildren(connections);

			// Building User Interfaces for room
			connectProTheme.BuildUserInterfaces();
		}

		public override void Dispose()
		{
			var core = m_Core as Core;
			core?.Dispose();

			ServiceProvider.RemoveService(m_Core);
		}
	}
}
