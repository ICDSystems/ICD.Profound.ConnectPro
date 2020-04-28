using System;

namespace ICD.Profound.ConnectPRO.Dialing.ConferenceSetup
{
	public interface IConferenceSetup : IDisposable
	{
		/// <summary>
		/// Raised when conference setup finishes.
		/// </summary>
		event EventHandler OnFinished;

		/// <summary>
		/// Starts the conference setup process.
		/// </summary>
		void Start();

		/// <summary>
		/// Ends the conference setup process and executes the finish action.
		/// </summary>
		void Finish();
	}
}
