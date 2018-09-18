using System;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Themes
{
	public interface IUserInterface : IDisposable
	{
		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		void SetRoom(IConnectProRoom room);

		/// <summary>
		/// Gets the room attached to this UI.
		/// </summary>
		IConnectProRoom Room { get; }

		/// <summary>
		/// Gets the target instance attached to this UI.
		/// </summary>
		object Target { get; }
	}
}
