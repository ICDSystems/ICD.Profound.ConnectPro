using System;
using ICD.Common.Utils.Timers;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPROCommon.Routing
{
	public sealed class ProcessingSourceInfo : IDisposable
	{
		public enum eProcessingState
		{
			None,
			Routing,
			Unrouting,
			Failed
		}

		/// <summary>
		/// Timeout processing sources after 5 seconds.
		/// </summary>
		private const long TIMEOUT_INTERVAL = 5 * 1000;

		private readonly IDestinationBase m_Destination;
		private readonly SafeTimer m_Timer;

		private Action m_Callback;

		/// <summary>
		/// Gets/sets the source.
		/// </summary>
		public ISource Source { get; set; }

		/// <summary>
		/// Gets the destination.
		/// </summary>
		public IDestinationBase Destination { get { return m_Destination; } }

		/// <summary>
		/// Gets/sets the state.
		/// </summary>
		public eProcessingState State { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="timerCallback"></param>
		public ProcessingSourceInfo(IDestinationBase destination, Action timerCallback)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			if (timerCallback == null)
				throw new ArgumentNullException("timerCallback");

			m_Destination = destination;
			m_Callback = timerCallback;

			m_Timer = SafeTimer.Stopped(TimerCallback);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			m_Timer.Dispose();
			m_Callback = null;
		}

		/// <summary>
		/// Starts the timer for the callback.
		/// </summary>
		public void ResetTimer()
		{
			m_Timer.Reset(TIMEOUT_INTERVAL);
		}

		/// <summary>
		/// Called when the timer elapses.
		/// </summary>
		private void TimerCallback()
		{
			Clear();
			m_Callback();
		}

		/// <summary>
		/// Clears the source, state, and stops the timer.
		/// </summary>
		public void Clear()
		{
			m_Timer.Stop();

			Source = null;
			State = eProcessingState.None;
		}
	}
}
