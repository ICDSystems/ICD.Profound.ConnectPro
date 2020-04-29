using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Routing.Masking.ConferenceDevice
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
