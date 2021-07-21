using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Profound.ConnectPROCommon.Devices;
using ICD.Profound.ConnectPROCommon.EventArguments;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Themes.EventServerUserInterface.EventServerKeyHandlers
{
	public sealed class VideoSourcesEventServerKeyHandler : AbstractEventServerKeyHandler
	{
		/// <summary>
		/// Gets the key for the message handler.
		/// </summary>
		public override string Key { get { return ConnectProEventMessages.KEY_VIDEO_SOURCES; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="device"></param>
		public VideoSourcesEventServerKeyHandler(IConnectProTheme theme, ConnectProEventServerDevice device)
			: base(theme, device)
		{
			Update();
		}

		/// <summary>
		/// Updates the message.
		/// </summary>
		public override void Update()
		{
			base.Update();

			ISource[] videoSources =
				Room == null
					? new ISource[0]
					: Room.Routing
					      .State
					      .GetFakeActiveVideoSources()
					      .SelectMany(kvp => kvp.Value)
					      .Distinct()
					      .ToArray();

			bool combined = Room != null && (Room.CombineState || Room.IsCombineRoom());

			Message =
				videoSources.Length == 0
					? "no sources routed for video"
					: string.Format("sources routed for video {0}",
									StringUtils.ArrayFormat(videoSources.Select(s => s.GetName(combined))));
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

			room.Routing.State.OnSourceRoutedChanged += RoomRoutingStateOnSourceRoutedChanged;
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

			room.Routing.State.OnSourceRoutedChanged += RoomRoutingStateOnSourceRoutedChanged;
		}

		/// <summary>
		/// Called when a source is routed/unrouted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RoomRoutingStateOnSourceRoutedChanged(object sender, SourceRoutedEventArgs e)
		{
			Update();
		}

		#endregion
	}
}
