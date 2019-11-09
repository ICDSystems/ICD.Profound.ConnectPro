using ICD.Profound.ConnectPRO.Rooms;

namespace ICD.Profound.ConnectPRO.Routing.Masking.ConferenceDevice
{
	public sealed class ConferenceDeviceMaskedSourceInfo : AbstractConferenceDeviceMaskedSourceInfo
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		public ConferenceDeviceMaskedSourceInfo(IConnectProRoom room)
			: base(room)
		{
		}
	}
}
