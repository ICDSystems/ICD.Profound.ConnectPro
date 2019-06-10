using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing.Masking
{
	public class MaskedSourceInfoFactory
	{
		private readonly IConnectProRoom m_Room;

		public IConnectProRoom Room
		{
			get { return m_Room; }
		}

		public MaskedSourceInfoFactory(IConnectProRoom room)
		{
			m_Room = room;
		}

		public IMaskedSourceInfo GetMaskedSourceInfo(ISource source)
		{
			if (Room.Core.Originators.GetChild(source.Device) is ZoomRoom)
				return new ConferenceDeviceMaskedSourceInfo(source, Room);

			return null;
		}
	}
}