using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
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
		private const ushort GRAPH_MINUM_POSITON_FROM_END = 1310;
		private const int DURATION_MINIMUM_VISIBLE = 2 * 1000; //Don't show graph <2 second times;

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
		private DateTime m_PowerStateChangedTime;
		private int m_PowerStateExpectedDuration;
		private IPowerDeviceControl m_ActivePowerControl;

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

		public string PowerStateText { get; private set; }

		public bool ShowStatusGauge { get { return (m_PowerState == ePowerState.Warming || m_PowerState == ePowerState.Cooling) && m_PowerStateExpectedDuration >= DURATION_MINIMUM_VISIBLE; } }

		public ushort DurationGraphValue
		{
			get
			{
				int? timeRemaining = GetTimeRemaining();

				if (!timeRemaining.HasValue)
					return 0;
				if (timeRemaining.Value <= 0)
				{
					if (m_PowerState == ePowerState.Warming)
						return ushort.MaxValue - GRAPH_MINUM_POSITON_FROM_END;

					return GRAPH_MINUM_POSITON_FROM_END;
				}

				IcdConsole.PrintLine(eConsoleColor.Magenta, "DurationGraphValue Calc: TimeRemaining {0}", timeRemaining.Value);

				float graphPosition = (float)timeRemaining.Value / m_PowerStateExpectedDuration;

				//If warming, flip to incresing
				if (m_PowerState == ePowerState.Warming)
					graphPosition = 1 - graphPosition;


				// Max value is Expired value
				return MathUtils.Clamp((ushort)(graphPosition * ushort.MaxValue), GRAPH_MINUM_POSITON_FROM_END, (ushort)(ushort.MaxValue - GRAPH_MINUM_POSITON_FROM_END));

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

			Subscribe(m_Destination);

			UpdateLabels();
		}

		#region Methods

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

		/// <summary>
		/// Gets expected time remaining in milliseconds
		/// If time is not avaliable, returns null
		/// If time is past due, returns 0
		/// </summary>
		/// <returns></returns>
		public int? GetTimeRemaining()
		{
			return GetTimeRemaining(m_PowerStateChangedTime, m_PowerStateExpectedDuration);
		}

		private int? GetTimeRemaining(DateTime startTime, int expectedDuration)
		{
			if (expectedDuration == 0)
				return null;
			int runningTime = GetMillisecondsSince(startTime);

			//todo: Remove Debug
			IcdConsole.PrintLine(eConsoleColor.Magenta, "GetTimeRemaining DT calc: runningTime TotalMs {0}", runningTime);

			if (runningTime >= expectedDuration)
				return 0;

			int remainingTime = expectedDuration - runningTime;

			return remainingTime;
		}

		private int GetMillisecondsSince(DateTime time)
		{
			return (int)DateTime.UtcNow.Subtract(time).TotalMilliseconds;
		}

		#endregion

		#region Private Methods

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

		private void UpdatePowerStateLabel()
		{
			switch (m_PowerState)
			{
				case ePowerState.Warming:
					PowerStateText = "Warming";
					break;
				case ePowerState.Cooling:
					PowerStateText = "Cooling";
					break;
				default:
					PowerStateText = string.Empty;
					break;
			}
		}

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			Unsubscribe(m_Destination);
		}

		#endregion

		#region Destination Callbacks

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

		private void Unsubscribe(IDestinationBase destinationBase)
		{
			if (destinationBase == null)
				return;

			m_PowerStateCache.Keys.ForEach(Unsubscribe);
			m_PowerStateCache.Clear();
		}

		#endregion

		#region PowerDeviceControl callbacks

		private void Subscribe(IPowerDeviceControl powerControl)
		{
			if (powerControl == null)
				return;

			m_PowerStateCache[powerControl] = new DisplayPowerState(powerControl.PowerState);

			powerControl.OnPowerStateChanged += PowerControlOnPowerStateChanged;
		}

		private void Unsubscribe(IPowerDeviceControl powerControl)
		{
			if (powerControl == null)
				return;

			powerControl.OnPowerStateChanged -= PowerControlOnPowerStateChanged;
		}

		private void PowerControlOnPowerStateChanged(object sender, PowerDeviceControlPowerStateApiEventArgs args)
		{
			IPowerDeviceControl powerControl = sender as IPowerDeviceControl;

			if (powerControl == null)
				return;

			//Update the dictionary
			m_PowerStateCache[powerControl] = new DisplayPowerState(args.Data);

			// Is the control that we're currently using as the active one
			if (m_ActivePowerControl == powerControl)
			{
				// Check if the new state is same or higher priority, if so, update for new state
				// If not, recaculate power feedback from all the controls
				if(CompareState(args.Data.PowerState, m_PowerState) < 1)
					UpdatePowerState(powerControl);
				else
					RecaculatePowerFeedback();
			}
			else
			{
				int powerStateComparison = CompareState(args.Data.PowerState,m_PowerState);

				// If the priority is higher, or the priority is the same but the expected duration is longer, use this power control instead
				if (powerStateComparison == -1)
					UpdatePowerState(powerControl);
				else if (powerStateComparison == 0 && args.Data.ExpectedDuration > GetTimeRemaining())
					UpdatePowerState(powerControl);

			}
		}

		/// <summary>
		/// Update all the power state variables with the info from the given control
		/// </summary>
		/// <param name="control"></param>
		private void UpdatePowerState(IPowerDeviceControl control)
		{
			DisplayPowerState state;

			if (!m_PowerStateCache.TryGetValue(control, out state))
				return;

			m_ActivePowerControl = control;
			m_PowerStateChangedTime = state.EffectiveTime;
			m_PowerStateExpectedDuration = state.ExpectedDuration;
			m_PowerState = state.PowerState;

			UpdatePowerStateLabel();

			OnRefreshNeeded.Raise(this);
		}

		private void RecaculatePowerFeedback()
		{
			ePowerState currentState = ePowerState.Unknown;
			int currentRemaining = 0;
			IPowerDeviceControl control = null;

			foreach (var kvp in m_PowerStateCache)
			{
				int stateCompare = CompareState(kvp.Value.PowerState, currentState);
				int? itemTimeRemaining = GetTimeRemaining(kvp.Value.EffectiveTime, kvp.Value.ExpectedDuration);
				if (stateCompare == -1 || (stateCompare == 0 && (itemTimeRemaining.GetValueOrDefault() > currentRemaining)))
				{
					control = kvp.Key;
					currentRemaining = itemTimeRemaining.GetValueOrDefault();
					currentState = kvp.Value.PowerState;
				}
			}

			UpdatePowerState(control);
		}

		public static int CompareState(ePowerState x, ePowerState y)
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

		private sealed class DisplayPowerState
		{


			private readonly DateTime m_EffectiveTime;

			private readonly PowerDeviceControlPowerStateEventData m_StateData;

			public DateTime EffectiveTime { get { return m_EffectiveTime; } }

			public ePowerState PowerState { get { return m_StateData.PowerState; } }

			public int ExpectedDuration { get { return m_StateData.ExpectedDuration; } }

			public DisplayPowerState(PowerDeviceControlPowerStateEventData stateData)
			{
				m_StateData = stateData;
				m_EffectiveTime = DateTime.UtcNow;
			}

			public DisplayPowerState(ePowerState powerState) : this(new PowerDeviceControlPowerStateEventData(powerState))
			{
			}

		}
	}

	
}
