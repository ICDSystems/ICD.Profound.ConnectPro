using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom.Controls.Conferencing;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Dialing.ConferenceSetup
{
	public static class ConferenceSetupFactory
	{
		private static readonly Dictionary<Type, Func<IConnectProRoom, IConferenceDeviceControl, Action, IConferenceSetup>>
			s_Factories = new Dictionary<Type, Func<IConnectProRoom, IConferenceDeviceControl, Action, IConferenceSetup>>
			{
				// Zoom
				{typeof(ZoomRoomConferenceControl), (room, control, action) => new ZoomConferenceSetup(room, control, action)},
				{typeof(ZoomRoomTraditionalConferenceControl), (room, control, action) => new ZoomConferenceSetup(room, control, action)},
			};

		/// <summary>
		/// Builds a new conference setup instance for the given conference control and finish setup action.
		/// Returns null if the conference control does not require setup.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="control"></param>
		/// <param name="finishAction"></param>
		/// <returns></returns>
		[CanBeNull]
		public static IConferenceSetup BuildConferenceSetup([NotNull] IConnectProRoom room,
		                                                    [NotNull] IConferenceDeviceControl control,
		                                                    [NotNull] Action finishAction)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			if (control == null)
				throw new ArgumentNullException("control");

			if (finishAction == null)
				throw new ArgumentNullException("finishAction");

			Func<IConnectProRoom, IConferenceDeviceControl, Action, IConferenceSetup> factory;
			return s_Factories.TryGetValue(control.GetType(), out factory)
				       ? factory(room, control, finishAction)
				       : null;
		}
	}
}
