using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.SettingsTree.TouchFreeConferencing
{
	public sealed class TouchFreeSettingsLeaf : AbstractSettingsLeaf
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		public TouchFreeSettingsLeaf()
		{
			Name = "Touch Free Conferencing";
			Icon = eSettingsIcon.TouchFree;
		}

		/// <summary>
		/// Override to initialize the node once a room has been assigned.
		/// </summary>
		protected override void Initialize(IConnectProRoom room)
		{
			base.Initialize(room);
		}

		
		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Unsubscribe(room);

			if (room == null)
				return;
		}
		#endregion
	}
}