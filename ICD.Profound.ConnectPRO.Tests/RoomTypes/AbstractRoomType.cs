﻿using System;
using ICD.Connect.Panels.Mock;
using ICD.Connect.Settings.Core;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Tests.RoomTypes.Common
{
    public abstract class AbstractRoomType : IDisposable
    {
	    public abstract ConnectProRoom Room { get; }

		public abstract MockPanelDevice Panel { get; }

		public abstract ICore Core { get; }

	    public abstract void Dispose();
    }
}
