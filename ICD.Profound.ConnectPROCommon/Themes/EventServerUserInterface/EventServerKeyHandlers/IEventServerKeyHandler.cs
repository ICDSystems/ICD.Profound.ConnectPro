using System;
using ICD.Common.Properties;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public interface IEventServerKeyHandler : IDisposable
	{
		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		string Key { get; }

		/// <summary>
		/// Gets the current message.
		/// </summary>
		string Message { get; }

		/// <summary>
		/// Gets the room.
		/// </summary>
		IConnectProRoom Room { get; }

		/// <summary>
		/// Gets the theme.
		/// </summary>
		IConnectProTheme Theme { get; }

		/// <summary>
		/// Sets the room.
		/// </summary>
		/// <param name="value"></param>
		void SetRoom([CanBeNull] IConnectProRoom value);

		/// <summary>
		/// Pushes the current state to the device.
		/// </summary>
		void Update();
	}
}