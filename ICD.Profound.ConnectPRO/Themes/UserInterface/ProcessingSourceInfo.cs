using System;
using ICD.Common.Utils.Timers;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface
{
	public sealed class ProcessingSourceInfo : IDisposable
	{
		/// <summary>
		/// Timeout processing sources after 5 seconds.
		/// </summary>
		private const long TIMEOUT_INTERVAL = 5 * 1000;

		private readonly IDestination m_Destination;
		private readonly SafeTimer m_Timer;

		private Action<IDestination, ISource> m_Callback;

		/// <summary>
		/// Gets the source.
		/// </summary>
		public ISource Source { get; set; }

		/// <summary>
		/// Gets the destination.
		/// </summary>
		public IDestination Destination { get { return m_Destination; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="timerCallback"></param>
		public ProcessingSourceInfo(IDestination destination, Action<IDestination, ISource> timerCallback)
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
		/// Stops the timer for the callback.
		/// </summary>
		public void StopTimer()
		{
			m_Timer.Stop();
		}

		/// <summary>
		/// Called when the timer elapses.
		/// </summary>
		private void TimerCallback()
		{
			if (Source != null)
				m_Callback(m_Destination, Source);
		}
	}
}
