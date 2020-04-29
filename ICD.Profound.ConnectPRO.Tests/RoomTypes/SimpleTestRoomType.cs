using System.Collections.Generic;
using ICD.Common.Utils.Services;
using ICD.Connect.Audio.Shure.Devices.MXA;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.PartitionManagers;
using ICD.Connect.Routing.RoutingGraphs;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Themes.UserInterfaceFactories;
using ICD.Profound.ConnectPRO.Themes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Rooms.Single;

namespace ICD.Profound.ConnectPRO.Tests.RoomTypes
{
	public sealed class SimpleTestRoomType : AbstractRoomType
	{
		private readonly IConnectProRoom m_Room;
		private readonly MockPanelDevice m_Panel;
		private readonly ICore m_Core;

		public override IConnectProRoom Room { get { return m_Room; } }
		public override MockPanelDevice Panel { get { return m_Panel; } }
		public override ICore Core { get { return m_Core; } }

		public IConnectProRoom ConnectProRoom1 { get; set; }
		public IConnectProRoom ConnectProRoom2 { get; set; }
		public ConnectProTheme ConnectProTheme { get; set; }
		public MockPanelDevice MockPanelDevice { get; set; }
		public ShureMxa910Device MicrophoneDevice { get; set; }
		public OsdPanelDevice OsdDevice { get; set; }

		public SimpleTestRoomType()
		{
			m_Core = new Core {Id = 100};

			// Instantiating RoutingGraph and adding to core
			var routingGraph = new RoutingGraph {Id = 200};
			m_Core.Originators.AddChild(routingGraph);

			// Instantiating PartitionManager and adding to core
			var partitionManager = new PartitionManager { Id = 201 };
			m_Core.Originators.AddChild(partitionManager);

			// Instantiating rooms and devices
			ConnectProTheme = new ConnectProTheme { Id = 300 };
			ConnectProRoom1 = new ConnectProRoom { Id = 400 };
			ConnectProRoom2 = new ConnectProRoom { Id = 500 };
			MockPanelDevice = new MockPanelDevice { Id = 600 };
			MicrophoneDevice = new ShureMxa910Device { Id = 700 };
			OsdDevice = new OsdPanelDevice { Id = 800 };

			// Adding rooms and devices to core Originators
			m_Core.Originators.AddChild(ConnectProRoom1);
			m_Core.Originators.AddChild(ConnectProRoom2);
			m_Core.Originators.AddChild(MockPanelDevice);
			m_Core.Originators.AddChild(ConnectProTheme);
			m_Core.Originators.AddChild(MicrophoneDevice);
			m_Core.Originators.AddChild(OsdDevice);

			// Building User Interfaces for empty rooms
			ConnectProTheme.BuildUserInterfaces();
		}

		public override void Dispose()
		{
			var core = m_Core as Core;
			core?.Dispose();

			ServiceProvider.RemoveService(m_Core);
		}

		public IEnumerable<IUserInterfaceFactory> GetInterfaces()
		{
			return ConnectProTheme.GetUiFactories();
		}
	}
}
