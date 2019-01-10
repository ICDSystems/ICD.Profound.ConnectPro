using System;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Settings.Cores;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Tests.RoomTypes
{
    public abstract class AbstractRoomType : IDisposable
    {
	    public abstract ConnectProRoom Room { get; }

		public abstract MockPanelDevice Panel { get; }

		public abstract ICore Core { get; }

		public abstract void Dispose();
    }
}
