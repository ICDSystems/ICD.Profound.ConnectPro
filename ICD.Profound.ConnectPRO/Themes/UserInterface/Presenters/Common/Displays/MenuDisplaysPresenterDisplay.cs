using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Timers;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuDisplaysPresenterDisplay : IDisposable
	{
		private const int MAX_LINE_WIDTH = 20;
		private const ushort GRAPH_MINIMUM_POSITION_FROM_END = 1310;
		private const int DURATION_MINIMUM_VISIBLE = 2 * 1000; // Don't show graph <2 second times;

		private readonly IConnectProRoom m_Room;
		private readonly IDestinationBase m_Destination;
		private readonly bool m_RoomCombine;
		private readonly Dictionary<IPowerDeviceControl, DisplayPowerState> m_PowerStateCache;

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
		private readonly IcdStopwatch m_Stopwatch;

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

		public string PowerStateText
		{
			get
			{
				switch (m_PowerState)
				{
					case ePowerState.Warming:
						return "Warming";
					case ePowerState.Cooling:
						return "Cooling";
				}

				return string.Empty;
			}
		}

		public bool ShowStatusGauge
		{
			get
			{
				return (m_PowerState == ePowerState.Warming || m_PowerState == ePowerState.Cooling) &&
				       m_PowerStateExpectedDuration >= DURATION_MINIMUM_VISIBLE;
			}
		}

		public ushort DurationGraphValue
		{
			get
			{
				long timeRemaining = GetTimeRemaining();
				float graphPosition = (float)timeRemaining / m_PowerStateExpectedDuration;

				// If warming, flip to increasing
				if (m_PowerState == ePowerState.Warming)
					graphPosition = 1 - graphPosition;

				// Max value is Expired value
				return MathUtils.Clamp((ushort)(graphPosition * ushort.MaxValue),
				                       GRAPH_MINIMUM_POSITION_FROM_END,
				                       (ushort)(ushort.MaxValue - GRAPH_MINIMUM_POSITION_FROM_END));
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
			m_PowerStateCache = new Dictionary<IPowerDeviceControl, DisplayPowerState>();
			m_Stopwatch = new IcdStopwatch();

			Subscribe(m_Destination);

			UpdateLabels();
		}

		#region Methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			m_Stopwatch.Stop();
			Unsubscribe(m_Destination);
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
			return GetTimeRemaining(m_PowerStateExpectedDuration);
		}

		/// <summary>
		/// Gets expected time remaining in milliseconds.
		/// If time is past due, returns 0.
		/// </summary>
		/// <param name="expectedDuration"></param>
		/// <returns></returns>
		private long GetTimeRemaining(long expectedDuration)
		{
			long remaining = expectedDuration - m_Stopwatch.ElapsedMilliseconds;
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

			foreach (IDestination destination in destinationBase.GetDestinations())
			{
				IDeviceBase device = m_Room.Core.Originators.GetChild<IDeviceBase>(destination.Device);
				IPowerDeviceControl powerControl = device.Controls.GetControl<IPowerDeviceControl>();

				Subscribe(powerControl);
			}
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

			m_PowerStateCache[powerControl] = new DisplayPowerState(powerControl.PowerState);

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

			m_PowerStateCache[powerControl] = new DisplayPowerState(args.Data);

			ePowerState currentState = ePowerState.Unknown;
			long currentRemaining = 0;
			IPowerDeviceControl control = null;

			foreach (KeyValuePair<IPowerDeviceControl, DisplayPowerState> kvp in m_PowerStateCache)
			{
				int stateCompare = CompareState(kvp.Value.PowerState, currentState);
				long itemTimeRemaining = GetTimeRemaining(kvp.Value.ExpectedDuration);

				if (stateCompare != -1 && (stateCompare != 0 || itemTimeRemaining <= currentRemaining))
					continue;

				control = kvp.Key;
				currentRemaining = itemTimeRemaining;
				currentState = kvp.Value.PowerState;
			}

			UpdatePowerState(control);
		}

		/// <summary>
		/// Update all the power state variables with the info from the given control.
		/// </summary>
		/// <param name="control"></param>
		private void UpdatePowerState(IPowerDeviceControl control)
		{
			if (control == null)
				throw new ArgumentNullException("control");

			DisplayPowerState state;
			if (!m_PowerStateCache.TryGetValue(control, out state))
				return;

			m_PowerStateExpectedDuration = state.ExpectedDuration;
			m_PowerState = state.PowerState;

			// Get current elapsed time and start stopwatch
			long remainingTime = GetTimeRemaining();
			if (remainingTime > 0)
				m_Stopwatch.Restart();
			else
				m_Stopwatch.Stop();

			OnRefreshNeeded.Raise(this);
		}

		/// <summary>
		/// Returns 0 if states are the same.
		/// Returns -1 if x is more important than y.
		/// Returns 1 if x is less important than y.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private static int CompareState(ePowerState x, ePowerState y)
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

		#endregion

		private struct DisplayPowerState
		{
			private readonly ePowerState m_PowerState;
			private readonly long m_ExpectedDuration;

			public ePowerState PowerState { get { return m_PowerState; } }

			public long ExpectedDuration { get { return m_ExpectedDuration; } }

			public DisplayPowerState(PowerDeviceControlPowerStateEventData stateData)
				: this(stateData.PowerState, stateData.ExpectedDuration)
			{
			}

			public DisplayPowerState(ePowerState powerState)
				: this(powerState, 0)
			{
			}

			public DisplayPowerState(ePowerState powerState, long expectedDuration)
			{
				m_PowerState = powerState;
				m_ExpectedDuration = expectedDuration;
			}
		}
	}
}
