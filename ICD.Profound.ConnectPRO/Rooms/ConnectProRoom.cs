using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Services.Scheduler;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Audio.Controls;
using ICD.Connect.Audio.VolumePoints;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Favorites.SqLite;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Displays.Devices;
using ICD.Connect.Panels;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Settings.Core;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class ConnectProRoom : AbstractRoom<ConnectProRoomSettings>, IConnectProRoom
	{
		/// <summary>
		/// Raised when the room starts/stops a meeting.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		private readonly IConferenceManager m_ConferenceManager;
		private readonly ConnectProRouting m_Routing;

		private readonly WakeSchedule m_WakeSchedule;

		private bool m_IsInMeeting;
		private DialingPlanInfo m_DialingPlan;

		#region Properties

		/// <summary>
		/// Gets/sets the current meeting status.
		/// </summary>
		public bool IsInMeeting
		{
			get { return m_IsInMeeting; }
			private set
			{
				if (value == m_IsInMeeting)
					return;

				m_IsInMeeting = value;

				OnIsInMeetingChanged.Raise(this, new BoolEventArgs(m_IsInMeeting));
			}
		}

		/// <summary>
		/// Gets the routing features for this room.
		/// </summary>
		public ConnectProRouting Routing { get { return m_Routing; } }

		/// <summary>
		/// Gets the conference manager.
		/// </summary>
		public IConferenceManager ConferenceManager { get { return m_ConferenceManager; } }

		public WakeSchedule WakeSchedule { get { return m_WakeSchedule; } }

		public string Passcode { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoom()
		{
			m_Routing = new ConnectProRouting(this);
			m_ConferenceManager = new ConferenceManager();
			m_WakeSchedule = new WakeSchedule();
			Subscribe(m_WakeSchedule);
			ServiceProvider.TryGetService<IActionSchedulerService>().Add(m_WakeSchedule);
		}

		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_WakeSchedule);
			ServiceProvider.TryGetService<IActionSchedulerService>().Remove(m_WakeSchedule);
		}

		#region Methods

		/// <summary>
		/// Gets the volume control matching the configured volume point.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public IVolumeDeviceControl GetVolumeControl()
		{
			IVolumePoint volumePoint = Originators.GetInstance<IVolumePoint>();
			if (volumePoint == null)
				return null;

			try
			{
				return Core.GetControl<IVolumeDeviceControl>(volumePoint.DeviceId, volumePoint.ControlId);
			}
			catch (Exception)
			{
				Log(eSeverity.Error, "Failed to find volume control for {0}", volumePoint);
			}

			return null;
		}

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		public void StartMeeting()
		{
			StartMeeting(true);
		}

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		/// <param name="resetRouting"></param>
		public void StartMeeting(bool resetRouting)
		{
			if (resetRouting)
				Routing.RouteOsd();

			IsInMeeting = true;
		}

		/// <summary>
		/// Ends the meeting state. If shutdown is true fully powers down the room.
		/// </summary>
		/// <param name="shutdown"></param>
		public void EndMeeting(bool shutdown)
		{
			// Hangup
			if (ConferenceManager != null && ConferenceManager.ActiveConference != null)
				ConferenceManager.ActiveConference.Hangup();

			// Reset all routing
			Routing.RouteOsd();

			if (shutdown)
				Sleep();

			IsInMeeting = false;
		}

		/// <summary>
		/// Wakes up the room.
		/// </summary>
		public void Wake()
		{
			// Reset all routing
			Routing.RouteOsd();

			// Power the panels
			Originators.GetInstancesRecursive<IPanelDevice>()
					   .SelectMany(panel => panel.Controls.GetControls<IPowerDeviceControl>())
					   .ForEach(c => c.PowerOn());

			IsInMeeting = false;
		}

		/// <summary>
		/// Shuts down the room.
		/// </summary>
		public void Sleep()
		{
			// Hangup
			if (ConferenceManager != null && ConferenceManager.ActiveConference != null)
				ConferenceManager.ActiveConference.Hangup();

			// Reset all routing
			Routing.RouteOsd();

			// Power off displays
			foreach (IDestination destination in Routing.GetDisplayDestinations())
			{
				IDisplay display = Core.Originators.GetChild(destination.Device) as IDisplay;
				if (display != null)
					display.PowerOff();
			}

			// Power off the panels
			Originators.GetInstancesRecursive<IPanelDevice>()
			           .SelectMany(panel => panel.Controls.GetControls<IPowerDeviceControl>())
			           .ForEach(c => c.PowerOff());

			IsInMeeting = false;
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(ConnectProRoomSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.DialingPlan = m_DialingPlan;
			settings.Passcode = Passcode;

			settings.WakeSchedule.Copy(m_WakeSchedule);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_DialingPlan = default(DialingPlanInfo);

			m_ConferenceManager.ClearDialingProviders();
			m_ConferenceManager.Favorites = null;
			m_ConferenceManager.DialingPlan.ClearMatchers();

			Passcode = null;

			m_WakeSchedule.Clear();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(ConnectProRoomSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			// Dialing plan
			SetDialingPlan(settings.DialingPlan, factory);

			// Favorites
			string path = PathUtils.GetProgramConfigPath("favorites");
			m_ConferenceManager.Favorites = new SqLiteFavorites(path);

			// Passcode
			Passcode = settings.Passcode;

			// Wake Schedule
			m_WakeSchedule.Copy(settings.WakeSchedule);
		}

		/// <summary>
		/// Sets the dialing plan from the settings.
		/// </summary>
		/// <param name="planInfo"></param>
		/// <param name="factory"></param>
		private void SetDialingPlan(DialingPlanInfo planInfo, IDeviceFactory factory)
		{
			m_DialingPlan = planInfo;

			// TODO - Move loading from path into the DialingPlan.
			string dialingPlanPath = string.IsNullOrEmpty(m_DialingPlan.ConfigPath)
										 ? null
										 : PathUtils.GetDefaultConfigPath("DialingPlans", m_DialingPlan.ConfigPath);

			try
			{
				if (string.IsNullOrEmpty(dialingPlanPath))
					Log(eSeverity.Warning, "No Dialing Plan configured");
				else
				{
					string xml = IcdFile.ReadToEnd(dialingPlanPath, new UTF8Encoding(false));
					xml = EncodingUtils.StripUtf8Bom(xml);

					m_ConferenceManager.DialingPlan.LoadMatchersFromXml(xml);
				}
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "failed to load Dialing Plan {0} - {1}", dialingPlanPath, e.Message);
			}

			// If there are no audio or video providers, search the available controls
			if (m_DialingPlan.VideoEndpoint.DeviceId == 0 && m_DialingPlan.AudioEndpoint.DeviceId == 0)
			{
				IDialingDeviceControl[] dialers = this.GetControlsRecursive<IDialingDeviceControl>().ToArray();

				DeviceControlInfo video = dialers.Where(d => d.Supports == eConferenceSourceType.Video)
												 .Select(d => d.DeviceControlInfo)
												 .FirstOrDefault();

				DeviceControlInfo audio = dialers.Where(d => d.Supports == eConferenceSourceType.Audio)
												 .Select(d => d.DeviceControlInfo)
												 .FirstOrDefault(video);

				m_DialingPlan = new DialingPlanInfo(m_DialingPlan.ConfigPath, video, audio);
			}

			// Setup the dialing providers
			if (m_DialingPlan.VideoEndpoint.DeviceId != 0)
				TryRegisterDialingProvider(m_DialingPlan.VideoEndpoint, eConferenceSourceType.Video, factory);

			if (m_DialingPlan.AudioEndpoint.DeviceId != 0)
				TryRegisterDialingProvider(m_DialingPlan.AudioEndpoint, eConferenceSourceType.Audio, factory);
		}

		private void TryRegisterDialingProvider(DeviceControlInfo info, eConferenceSourceType sourceType, IDeviceFactory factory)
		{
			try
			{
				IDeviceBase device = factory.GetOriginatorById<IDeviceBase>(info.DeviceId);
				IDialingDeviceControl control = device.Controls.GetControl<IDialingDeviceControl>(info.ControlId);

				m_ConferenceManager.RegisterDialingProvider(sourceType, control);
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "failed add {0} dialing provider - {1}", sourceType, e.Message);
			}
		}

		#endregion

		#region WakeSchedule Callbacks

		private void Subscribe(WakeSchedule schedule)
		{
			schedule.OnWakeActionRequested += ScheduleOnWakeActionRequested;
			schedule.OnSleepActionRequested += ScheduleOnSleepActionRequested;
		}

		private void Unsubscribe(WakeSchedule schedule)
		{
			schedule.OnWakeActionRequested -= ScheduleOnWakeActionRequested;
			schedule.OnSleepActionRequested -= ScheduleOnSleepActionRequested;
		}

		private void ScheduleOnSleepActionRequested(object sender, EventArgs eventArgs)
		{
			Log(eSeverity.Informational, "Scheduled sleep occurring at {0}", IcdEnvironment.GetLocalTime().ToShortTimeString());
			Sleep();
		}

		private void ScheduleOnWakeActionRequested(object sender, EventArgs eventArgs)
		{
			Log(eSeverity.Informational, "Scheduled wake occurring at {0}", IcdEnvironment.GetLocalTime().ToShortTimeString());
			Wake();
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Is In Meeting", IsInMeeting);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("Start Meeting", "Starts the meeting", () => StartMeeting());
			yield return new ConsoleCommand("End Meeting", "Ends the meeting", () => EndMeeting(false));
			yield return new ConsoleCommand("Wake", "Wakes the room", () => Wake());
			yield return new ConsoleCommand("Sleep", "Puts the room to sleep", () => Sleep());
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
