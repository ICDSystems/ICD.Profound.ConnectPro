using System;
using System.Linq;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Services.Scheduler;
using ICD.Connect.Calendaring.Controls;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.ConferencePoints;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Favorites.SqLite;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Utils;

namespace ICD.Profound.ConnectPRO.Rooms.Single
{
	public sealed class ConnectProRoom : AbstractConnectProRoom<ConnectProRoomSettings>
	{
		private readonly IConferenceManager m_ConferenceManager;
		private readonly WakeSchedule m_WakeSchedule;

		private string m_DialingPlan;
		private ICalendarControl m_CalendarControl;

		#region Properties

		/// <summary>
		/// Gets the scheduler service.
		/// </summary>
		private IActionSchedulerService SchedulerService
		{
			get { return ServiceProvider.TryGetService<IActionSchedulerService>(); }
		}

		/// <summary>
		/// Gets the conference manager.
		/// </summary>
		public override IConferenceManager ConferenceManager { get { return m_ConferenceManager; } }

		/// <summary>
		/// Gets the wake/sleep schedule.
		/// </summary>
		public override WakeSchedule WakeSchedule { get { return m_WakeSchedule; } }

		/// <summary>
		/// Gets/sets the passcode for the settings page.
		/// </summary>
		public override string Passcode { get; set; }

		/// <summary>
		/// Gets/sets the ATC number for dialing into the room.
		/// </summary>
		public override string AtcNumber { get; set; }

		/// <summary>
		/// Gets/sets the calendar control for the room.
		/// </summary>
		public override ICalendarControl CalendarControl { get { return m_CalendarControl; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoom()
		{
			m_ConferenceManager = new ConferenceManager();
			m_WakeSchedule = new WakeSchedule();

			Subscribe(m_WakeSchedule);

			SchedulerService.Add(m_WakeSchedule);
		}

		/// <summary>
		/// Release resources
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_WakeSchedule);

			SchedulerService.Remove(m_WakeSchedule);
		}

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

			if (CalendarControl != null && CalendarControl.Parent != null)
				settings.CalendarDevice = CalendarControl.Parent.Id;

			settings.WakeSchedule.Copy(m_WakeSchedule);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_DialingPlan = null;

			m_ConferenceManager.ClearDialingProviders();
			m_ConferenceManager.Favorites = null;
			m_ConferenceManager.DialingPlan.ClearMatchers();

			AtcNumber = null;
			Passcode = null;
			m_CalendarControl = null;

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
			SetDialingPlan(settings.DialingPlan);

			// Favorites
			string path = PathUtils.GetProgramConfigPath("favorites");
			m_ConferenceManager.Favorites = new SqLiteFavorites(path);

			// ATC Number
			AtcNumber = settings.AtcNumber;

			// Passcode
			Passcode = settings.Passcode;

			// Calendar Device
			if (settings.CalendarDevice != null)
			{
				var calendarDevice = factory.GetOriginatorById<IDevice>(settings.CalendarDevice.Value);
				if (calendarDevice != null)
					m_CalendarControl = calendarDevice.Controls.GetControl<ICalendarControl>();
			}

			// Wake Schedule
			m_WakeSchedule.Copy(settings.WakeSchedule);

			// Generate conference points
			GenerateConferencePoints(factory);
		}

		/// <summary>
		/// We need to automatically generate conference points for sources with conferencing controls
		/// that do not already have conference points.
		/// 
		/// TODO - This assumes each device has only one conference control
		/// </summary>
		/// <param name="factory"></param>
		private void GenerateConferencePoints(IDeviceFactory factory)
		{
			foreach (ISource source in Originators.GetInstances<ISource>())
			{
				// Does the source have a conference control?
				IDeviceBase device = Core.Originators.GetChild<IDeviceBase>(source.Device);
				IConferenceDeviceControl control = device.Controls.GetControl<IConferenceDeviceControl>();
				if (control == null)
					continue;

				// Does the control have a conference point?
				if (Originators.GetInstances<IConferencePoint>().Any(c => c.DeviceId == device.Id))
					continue;

				int id = IdUtils.GetNewId(Core.Originators.GetChildrenIds().Concat(factory.GetOriginatorIds()),
				                          IdUtils.GetSubsystemId(IdUtils.SUBSYSTEM_POINTS),
				                          Id);
				eCombineMode combineMode = Originators.GetCombineMode(source.Id);

				ConferencePoint point = new ConferencePoint
				{
					Id = id,
					Name = control.Name,
					DeviceId = device.Id,
					ControlId = control.Id,
					Type = control.Supports
				};

				Core.Originators.AddChild(point);
				Originators.Add(id, combineMode);

				m_ConferenceManager.RegisterDialingProvider(point);
			}
		}

		/// <summary>
		/// Sets the dialing plan from the settings.
		/// </summary>
		/// <param name="path"></param>
		private void SetDialingPlan(string path)
		{
			m_DialingPlan = path;

			if (!string.IsNullOrEmpty(path))
				path = PathUtils.GetDefaultConfigPath("DialingPlans", path);

			try
			{
				if (string.IsNullOrEmpty(path))
					Log(eSeverity.Warning, "No Dialing Plan configured");
				else
				{
					string xml = IcdFile.ReadToEnd(path, new UTF8Encoding(false));
					xml = EncodingUtils.StripUtf8Bom(xml);

					m_ConferenceManager.DialingPlan.LoadMatchersFromXml(xml);
				}
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "failed to load Dialing Plan {0} - {1}", path, e.Message);
			}

			foreach (IConferencePoint conferencePoint in Originators.GetInstancesRecursive<IConferencePoint>())
				m_ConferenceManager.RegisterDialingProvider(conferencePoint);
		}

		#endregion
	}
}
