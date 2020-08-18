using System;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.Calendaring.Bookings;
using ICD.Connect.Cameras.Devices;
using ICD.Connect.Conferencing.Controls.Routing;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning.Commercial.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Profound.ConnectPROCommon.Dialing;
using ICD.Profound.ConnectPROCommon.Routing;

namespace ICD.Profound.ConnectPROCommon.Rooms
{
	public interface IConnectProRoom : ICommercialRoom
	{
		/// <summary>
		/// Raised when the room starts/stops a meeting.
		/// </summary>
		event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		/// <summary>
		/// Raised when the source that is currently the primary focus of the room (i.e. VTC) changes.
		/// </summary>
		event EventHandler<SourceEventArgs> OnFocusSourceChanged;

		/// <summary>
		/// Raised when the active camera for the room changes.
		/// </summary>
		event EventHandler<GenericEventArgs<IDeviceBase>> OnActiveCameraChanged;

		#region Properties

		/// <summary>
		/// Gets/sets the current meeting status.
		/// </summary>
		bool IsInMeeting { get; }

		/// <summary>
		/// Gets the routing features for this room.
		/// </summary>
		ConnectProRouting Routing { get; }

		/// <summary>
		/// Gets the dialing features for this room.
		/// </summary>
		ConnectProDialing Dialing { get; }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		string Passcode { get; set; }

		/// <summary>
		/// Gets the selected OBTP booking.
		/// </summary>
		IBooking CurrentBooking { get; }

		///<summary>
		/// Gets the timer for grace period before automatically starting a meeting.
		/// </summary>
		IcdTimer MeetingStartTimer { get; }

		/// <summary>
		/// Gets/sets the source that is currently the primary focus of the room (i.e. VTC).
		/// </summary>
		[CanBeNull]
		ISource FocusSource { get; set; }

		/// <summary>
		/// Gets the camera that is currently active for conferencing.
		/// </summary>
		IDeviceBase ActiveCamera { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		/// <param name="booking"></param>
		/// <param name="source"></param>
		void StartMeeting([CanBeNull] IBooking booking, [CanBeNull] ISource source);

		/// <summary>
		/// Ends the meeting state. If shutdown is true fully powers down the room.
		/// </summary>
		/// <param name="shutdown">true to fully power down room</param>
		void EndMeeting(bool shutdown);

		/// <summary>
		/// Ends the meeting state. If it is currently sleep time fully powers down the room.
		/// </summary>
		void EndMeeting();

		/// <summary>
		/// Sets the active camera for the room.
		/// If both the selected camera and routing control are not null, attempts to route the camera.
		/// </summary>
		/// <param name="activeCamera"></param>
		/// <param name="vtcDestinationControl"></param>
		void SetActiveCamera([CanBeNull] ICameraDevice activeCamera,
		                     [CanBeNull] IVideoConferenceRouteControl vtcDestinationControl);

		#endregion
	}
}
