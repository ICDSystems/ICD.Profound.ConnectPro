﻿using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Connect.Audio.Shure;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.PartitionManagers;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Settings.Core;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes;

namespace ICD.Profound.ConnectPRO.Tests.RoomTypes
{
	public sealed class SimpleTestRoomType : AbstractRoomType
	{
		private readonly ConnectProRoom m_room;
		private readonly MockPanelDevice m_panel;
		private readonly ICore m_core;
		private readonly IcdHashSet<IUserInterfaceFactory> m_interfaces;

		public override IConnectProRoom Room { get { return m_room; } }
		public override MockPanelDevice Panel { get { return m_panel; } }
		public override ICore Core { get { return m_core; } }
		public IcdHashSet<IUserInterfaceFactory> Interfaces { get { return m_interfaces; } }

		public ConnectProRoom ConnectProRoom1 { get; set; }
		public ConnectProRoom ConnectProRoom2 { get; set; }
		public ConnectProTheme ConnectProTheme { get; set; }
		public MockPanelDevice MockPanelDevice { get; set; }
		public ShureMxa910Device MicrophoneDevice { get; set; }
		public OsdPanelDevice OsdDevice { get; set; }

		public SimpleTestRoomType()
		{
			m_core = new Core {Id = 100};

			// Instantiating RoutingGraph and adding to core
			var routingGraph = new RoutingGraph {Id = 200};
			m_core.Originators.AddChild(routingGraph);

			// Instantiating PartitionManager and adding to core
			var partitionManager = new PartitionManager { Id = 201 };
			m_core.Originators.AddChild(partitionManager);

			// Instantiating rooms and devices
			ConnectProTheme = new ConnectProTheme { Id = 300 };
			ConnectProRoom1 = new ConnectProRoom { Id = 400 };
			ConnectProRoom2 = new ConnectProRoom { Id = 500 };
			MockPanelDevice = new MockPanelDevice { Id = 600 };
			MicrophoneDevice = new ShureMxa910Device { Id = 700 };
			OsdDevice = new OsdPanelDevice { Id = 800 };

			// Adding rooms and devices to core Originators
			m_core.Originators.AddChild(ConnectProRoom1);
			m_core.Originators.AddChild(ConnectProRoom2);
			m_core.Originators.AddChild(MockPanelDevice);
			m_core.Originators.AddChild(ConnectProTheme);
			m_core.Originators.AddChild(MicrophoneDevice);
			m_core.Originators.AddChild(OsdDevice);

			// Building User Interfaces for empty rooms
			ConnectProTheme.BuildUserInterfaces();

			m_interfaces = ConnectProTheme.GetUiFactories().ToIcdHashSet();
		}

		public override void Dispose()
		{
			var core = m_core as Core;
			core?.Dispose();

			ServiceProvider.RemoveService(m_core);
		}
	}
}
