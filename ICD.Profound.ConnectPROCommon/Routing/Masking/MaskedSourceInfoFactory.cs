using ICD.Connect.Conferencing.Zoom.Devices.ZoomRooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Routing.Masking.ConferenceDevice;

namespace ICD.Profound.ConnectPROCommon.Routing.Masking
{
	public sealed class MaskedSourceInfoFactory
	{
		private readonly IConnectProRoom m_Room;

		public IConnectProRoom Room { get { return m_Room; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public MaskedSourceInfoFactory(IConnectProRoom room)
		{
			m_Room = room;
		}

		public IMaskedSourceInfo GetMaskedSourceInfo(ISource source)
		{
			if (Room.Core.Originators.GetChild(source.Device) is ZoomRoom)
				return new ZoomConferenceDeviceMaskedSourceInfo(Room)
				{
					Source = source
				};

			return null;
		}
	}
}