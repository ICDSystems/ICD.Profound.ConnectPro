using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuDisplaysPresenterDisplay : IDisposable
	{
		private const int MAX_LINE_WIDTH = 20;
		private const float GRAPH_MINIMUM_POSITION_FROM_END = 0.01f;
		private const int DURATION_MINIMUM_VISIBLE = 2 * 1000; // Don't show graph <2 second times;

		private readonly IConnectProRoom m_Room;
		private readonly IDestinationBase m_Destination;
		private readonly bool m_RoomCombine;
		private readonly Dictionary<IPowerDeviceControl, ControlPowerState> m_PowerStateCache;

		private eDisplayColor m_Color;
		private string m_SourceName;
		private string m_Line1;
		private string m_Line2;
		private bool m_AudioActive;
		private ISource m_RoutedSource;
		private ISource m_SelectedSource;
		private bool m_HasControl;
		private bool m_ShowSpeaker;
		private bool m_CanRouteToRoomAudio;
		private string m_Icon;
		private bool m_CanRouteVideo;

		private ePowerState m_PowerState;
		private long m_PowerStateExpectedDuration;

		/// <summary>
		/// We use a stopwatch to count elapsed time because DateTime is only has second-precision on some Crestron platforms
		/// </summary>
		private readonly IcdStopwatch m_WarmupTimer;

		#region Events

		public event EventHandler OnRefreshNeeded;

		#endregion

		#region Properties

		public eDisplayColor Color { get { return m_Color; } }
		public string SourceName { get { return m_SourceName; } }
		public string Line1 { get { return m_Line1; } }
		public string Line2 { get { return m_Line2; } }

		public string Icon { get { return m_Icon; } }

		public bool ShowSpeaker { get { return m_ShowSpeaker; } }

		public bool AudioActive { get { return m_AudioActive; } }

		public IDestinationBase Destination { get { return m_Destination; } }

		public ISource RoutedSource { get { return m_RoutedSource; } }

		public ePowerState PowerState { get { return m_PowerState; } }

		public bool ShowPowerState
		{
			get
			{
				return (m_PowerState == ePowerState.Warming || m_PowerState == ePowerState.Cooling) &&
				       m_PowerStateExpectedDuration >= DURATION_MINIMUM_VISIBLE;
			}
		}

		public float PowerStatePercent
		{
			get
			{
				long timeRemaining = GetTimeRemaining();
				float percent = (float)timeRemaining / m_PowerStateExpectedDuration;

				// If warming, flip to increasing
				if (m_PowerState == ePowerState.Warming)
					percent = 1.0f - percent;

				// Max value is Expired value
				return MathUtils.Clamp(percent,
				                       GRAPH_MINIMUM_POSITION_FROM_END,
				                       1.0f - GRAPH_MINIMUM_POSITION_FROM_END);
			}
		}

		public string PowerStateText
		{
			get
			{
				if (!ShowPowerState)
					return string.Empty;

				string percent = PowerStatePercent.ToString("P0");
				string color =
					PowerState == ePowerState.Warming
						? Colors.COLOR_RED
						: Colors.COLOR_DARK_BLUE;

				return HtmlUtils.FormatColoredText(percent, color);
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public MenuDisplaysPresenterDisplay(IConnectProRoom room, IDestinationBase destination, bool combine)
		{
			m_Room = room;
			m_Destination = destination;
			m_Color = eDisplayColor.Grey;
			m_RoomCombine = combine;
			m_PowerStateCache = new Dictionary<IPowerDeviceControl, ControlPowerState>();
			m_WarmupTimer = new IcdStopwatch();

			Subscribe(m_Destination);

			// Update has control
			UpdateHasControl();

			// Update the color
			UpdateColor();

			// Update icon
			UpdateIcon();

			// Update the labels
			UpdateLabels();

			// Update show speaker
			UpdateShowSpeaker();
		}

		#region Methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Unsubscribe(m_Destination);

			m_WarmupTimer.Stop();
		}

		public bool SetRoutedSource(ISource routedSource, bool canRouteToRoomAudio)
		{
			if (routedSource == m_RoutedSource &&
			    canRouteToRoomAudio == m_CanRouteToRoomAudio)
				return false;

			m_RoutedSource = routedSource;
			m_CanRouteToRoomAudio = canRouteToRoomAudio;

			// Update has control
			UpdateHasControl();

			// Update the color
			UpdateColor();

			// Update icon
			UpdateIcon();

			// Update the labels
			UpdateLabels();

			// Update show speaker
			UpdateShowSpeaker();

			return true;
		}

		public bool SetSelectedSource(ISource selectedSource, bool canRouteVideo)
		{
			if (selectedSource == m_SelectedSource && canRouteVideo == m_CanRouteVideo)
				return false;

			m_SelectedSource = selectedSource;
			m_CanRouteVideo = canRouteVideo;

			// Update the color
			UpdateColor();

			// Update icon
			UpdateIcon();

			// Update the labels
			UpdateLabels();

			// Update show speaker
			UpdateShowSpeaker();

			return true;
		}

		public bool SetAudioActive(bool value)
		{
			if (value == m_AudioActive)
				return false;

			m_AudioActive = value;

			return true;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets expected time remaining in milliseconds.
		/// If time is past due, returns 0.
		/// </summary>
		/// <returns></returns>
		private long GetTimeRemaining()
		{
			long remaining = m_PowerStateExpectedDuration - m_WarmupTimer.ElapsedMilliseconds;
			return remaining < 0 ? 0 : remaining;
		}

		private void UpdateHasControl()
		{
			m_HasControl = m_RoutedSource != null && ConnectProRoutingSources.CanControl(m_RoutedSource);
		}

		private void UpdateColor()
		{
			m_Color = m_SelectedSource == null || m_SelectedSource == m_RoutedSource
				          ? m_RoutedSource == null
					            ? eDisplayColor.Grey
					            : m_HasControl
						              ? eDisplayColor.Green
						              : eDisplayColor.White
				          : m_CanRouteVideo
					            ? eDisplayColor.Yellow
					            : eDisplayColor.Grey;
		}

		private void UpdateIcon()
		{
			ConnectProSource source = m_RoutedSource as ConnectProSource;
			string icon = source == null ? null : source.Icon;
			m_Icon = Icons.GetDisplayIcon(icon, m_Color);
		}

		private void UpdateLabels()
		{
			if (m_HasControl)
			{
				m_Line1 = "PRESS FOR CONTROLS";
				m_Line2 = string.Empty;
			}
			else
			{
				string destinationName =
					m_Destination == null
						? string.Empty
						: m_Destination.GetName(m_RoomCombine) ?? string.Empty;

				string text = m_SelectedSource == null || m_SelectedSource == m_RoutedSource
					              ? destinationName
					              : m_CanRouteVideo
						              ? string.Format("PRESS TO SHOW SELECTION ON {0}", destinationName)
						              : string.Format("UNABLE TO SHOW SELECTION ON {0}", destinationName);
				text = text.ToUpper();

				if (text.Length <= MAX_LINE_WIDTH)
				{
					m_Line1 = text;
					m_Line2 = string.Empty;
				}
				else
				{
					// Find the space closest to the middle of the text and split.
					int middleIndex = text.Length / 2;
					int splitIndex = text.FindIndices(char.IsWhiteSpace).GetClosest(i => i - middleIndex);

					m_Line1 = text.Substring(0, splitIndex).Trim();
					m_Line2 = text.Substring(splitIndex + 1).Trim();
				}
			}

			m_SourceName = m_RoutedSource == null ? string.Empty : m_RoutedSource.GetName(m_RoomCombine);

			string hexColor = Colors.DisplayColorToTextColor(m_Color);
			m_SourceName = HtmlUtils.FormatColoredText(m_SourceName, hexColor);
			m_Line1 = HtmlUtils.FormatColoredText(m_Line1, hexColor);
			m_Line2 = HtmlUtils.FormatColoredText(m_Line2, hexColor);
		}

		private void UpdateShowSpeaker()
		{
			m_ShowSpeaker = m_CanRouteToRoomAudio &&
			                (m_SelectedSource == null || m_SelectedSource == m_RoutedSource) &&
			                m_RoutedSource != null &&
			                m_RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio);
		}

		/// <summary>
		/// Update all the power state variables with the info from the control with the longest expected duration.
		/// </summary>
		private void UpdatePowerState()
		{
			// Get the control with the longest expected duration.
			// Prefer warmup durations to cooldown durations.
			KeyValuePair<IPowerDeviceControl, ControlPowerState> first;
			bool any = m_PowerStateCache.OrderBy(kvp => kvp.Value.PowerState, PowerStateComparer.Instance)
			                            .ThenByDescending(kvp => kvp.Value.ExpectedDuration)
			                            .TryFirst(out first);

			m_PowerStateExpectedDuration = any ? first.Value.ExpectedDuration : 0;
			m_PowerState = any ? first.Value.PowerState : 0;

			// Get current elapsed time and start stopwatch
			long remainingTime = GetTimeRemaining();
			if (remainingTime > 0)
			{
				if (!m_WarmupTimer.IsRunning)
					m_WarmupTimer.Restart();
			}
			else
			{
				m_WarmupTimer.Reset();
			}

			OnRefreshNeeded.Raise(this);
		}

		#endregion

		#region Destination Callbacks

		/// <summary>
		/// Subscribe to the destination events.
		/// </summary>
		/// <param name="destinationBase"></param>
		private void Subscribe(IDestinationBase destinationBase)
		{
			if (destinationBase == null)
				return;

			destinationBase.GetDevices()
			               .SelectMany(d => d.Controls.GetControls<IPowerDeviceControl>())
			               .ForEach(Subscribe);
		}

		/// <summary>
		/// Unsubscribe from the destination events.
		/// </summary>
		/// <param name="destinationBase"></param>
		private void Unsubscribe(IDestinationBase destinationBase)
		{
			if (destinationBase == null)
				return;

			m_PowerStateCache.Keys.ForEach(Unsubscribe);
			m_PowerStateCache.Clear();
		}

		#endregion

		#region PowerDeviceControl callbacks

		/// <summary>
		/// Subscribe to the power control events.
		/// </summary>
		/// <param name="powerControl"></param>
		private void Subscribe(IPowerDeviceControl powerControl)
		{
			if (powerControl == null)
				return;

			powerControl.OnPowerStateChanged += PowerControlOnPowerStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the power control events.
		/// </summary>
		/// <param name="powerControl"></param>
		private void Unsubscribe(IPowerDeviceControl powerControl)
		{
			if (powerControl == null)
				return;

			powerControl.OnPowerStateChanged -= PowerControlOnPowerStateChanged;
		}

		/// <summary>
		/// Called when a power controls power state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void PowerControlOnPowerStateChanged(object sender, PowerDeviceControlPowerStateApiEventArgs args)
		{
			IPowerDeviceControl powerControl = sender as IPowerDeviceControl;
			if (powerControl == null)
				return;

			// Add the expected duration onto the end of the current elapsed time
			long expected = args.Data.ExpectedDuration + m_WarmupTimer.ElapsedMilliseconds;
			m_PowerStateCache[powerControl] = new ControlPowerState(args.Data.PowerState, expected);

			UpdatePowerState();
		}

		#endregion

		private sealed class PowerStateComparer : IComparer<ePowerState>
		{
			private static PowerStateComparer s_Instance;

			public static PowerStateComparer Instance
			{
				get { return s_Instance ?? (s_Instance = new PowerStateComparer()); }
			}

			/// <summary>
			/// Returns 0 if states are the same.
			/// Returns -1 if x is more important than y.
			/// Returns 1 if x is less important than y.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(ePowerState x, ePowerState y)
			{
				if (x == y)
					return 0;

				if (x == ePowerState.Warming)
					return -1;
				if (y == ePowerState.Warming)
					return 1;

				if (x == ePowerState.PowerOn)
					return -1;
				if (y == ePowerState.PowerOn)
					return 1;

				if (x == ePowerState.Cooling)
					return -1;
				if (y == ePowerState.Cooling)
					return 1;

				if (x == ePowerState.PowerOff)
					return -1;
				if (y == ePowerState.PowerOff)
					return 1;

				throw new ArgumentException();
			}
		}

		private struct ControlPowerState
		{
			private readonly ePowerState m_PowerState;
			private readonly long m_ExpectedDuration;

			public ePowerState PowerState { get { return m_PowerState; } }

			public long ExpectedDuration { get { return m_ExpectedDuration; } }

			public ControlPowerState(ePowerState powerState, long expectedDuration)
			{
				m_PowerState = powerState;
				m_ExpectedDuration = expectedDuration;
			}
		}
	}
}
