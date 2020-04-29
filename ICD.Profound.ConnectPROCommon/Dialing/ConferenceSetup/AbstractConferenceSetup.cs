using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPROCommon.Dialing.ConferenceSetup
{
	public abstract class AbstractConferenceSetup : IConferenceSetup
	{
		public event EventHandler OnFinished;

		private readonly IConnectProRoom m_Room;
		private readonly IConferenceDeviceControl m_Control;
		private readonly Action m_FinishAction;

		/// <summary>
		/// Gets the room for the conference.
		/// </summary>
		[NotNull]
		public IConnectProRoom Room { get { return m_Room; } }

		/// <summary>
		/// Gets the wrapped conference control.
		/// </summary>
		[NotNull]
		public IConferenceDeviceControl Control { get { return m_Control; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="room"></param>
		/// <param name="control"></param>
		/// <param name="finishAction"></param>
		protected AbstractConferenceSetup([NotNull] IConnectProRoom room, [NotNull] IConferenceDeviceControl control,
		                                  [NotNull] Action finishAction)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			if (control == null)
				throw new ArgumentNullException("control");

			if (finishAction == null)
				throw new ArgumentNullException("finishAction");

			m_Room = room;
			m_Control = control;
			m_FinishAction = finishAction;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public virtual void Dispose()
		{
			OnFinished = null;
		}

		/// <summary>
		/// Starts the conference setup process.
		/// </summary>
		public virtual void Start()
		{
			Room.Logger.AddEntry(eSeverity.Informational, "{0} - Starting setup of conference for {1}", Room, Control);
		}

		/// <summary>
		/// Ends the conference setup process and executes the finish action.
		/// </summary>
		public virtual void Finish()
		{
			Room.Logger.AddEntry(eSeverity.Informational, "{0} - Finished setup of conference for {1}", Room, Control);

			m_FinishAction();

			OnFinished.Raise(this);
		}
	}
}
