using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Conferences;
using ICD.Connect.Conferencing.Controls;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Conferencing.Favorites.SqLite;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Displays;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Server.Osd;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Settings.Core;
using ICD.Profound.ConnectPRO.Routing;

namespace ICD.Profound.ConnectPRO.Rooms
{
	public sealed class ConnectProRoom : AbstractRoom<ConnectProRoomSettings>, IConnectProRoom
	{
		public event EventHandler<BoolEventArgs> OnIsInMeetingChanged;

		private readonly IConferenceManager m_ConferenceManager;
		private readonly ConnectProRouting m_Routing;

		private bool m_IsInMeeting;
		private DialingPlanInfo m_DialingPlan;
		private VolumePoint m_VolumePoint;

		#region Properties

		/// <summary>
		/// Gets/sets the current meeting status.
		/// </summary>
		public bool IsInMeeting { get { return m_IsInMeeting; } }

		/// <summary>
		/// Gets the routing features for this room.
		/// </summary>
		public ConnectProRouting Routing { get { return m_Routing; } }

		/// <summary>
		/// Gets the conference manager.
		/// </summary>
		public IConferenceManager ConferenceManager { get { return m_ConferenceManager; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectProRoom()
		{
			m_Routing = new ConnectProRouting(this);
			m_ConferenceManager = new ConferenceManager();
		}

		#region Methods

		/// <summary>
		/// Gets the volume control matching the configured volume point.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public IVolumeDeviceControl GetVolumeControl()
		{
			return m_VolumePoint == null
				       ? null
				       : Core.GetControl<IVolumeDeviceControl>(m_VolumePoint.DeviceId, m_VolumePoint.ControlId);
		}

		/// <summary>
		/// Enters the meeting state.
		/// </summary>
		public void StartMeeting()
		{
			if (m_IsInMeeting)
				return;

			m_IsInMeeting = true;
			Routing.RouteOsd();

			OnIsInMeetingChanged.Raise(this, new BoolEventArgs(m_IsInMeeting));
		}

		/// <summary>
		/// Ends the meeting state. If shutdown is true fully powers down the room.
		/// </summary>
		/// <param name="shutdown"></param>
		public void EndMeeting(bool shutdown)
		{
			bool change = m_IsInMeeting;
			m_IsInMeeting = false;

			Shutdown(shutdown);

			if (change)
				OnIsInMeetingChanged.Raise(this, new BoolEventArgs(m_IsInMeeting));
		}

		#endregion

		/// <summary>
		/// Shuts down the room.
		/// </summary>
		private void Shutdown(bool powerOff)
		{
			// Undo all routing
			if (powerOff)
			{
				Routing.UnrouteAll();
			}
			else
			{
				Routing.UnrouteAllExceptOsd();
				Routing.RouteOsd();
			}
			
			// Hangup
			if (ConferenceManager != null && ConferenceManager.ActiveConference != null)
				ConferenceManager.ActiveConference.Hangup();

			// Power off displays
			foreach (IDestination destination in Routing.GetDisplayDestinations())
			{
				if (!powerOff)
				{
					EndpointInfo? endpoint = Routing.GetActiveVideoEndpoint(destination);
					OsdPanelDevice osd = endpoint == null ? null : Core.Originators.GetChild(endpoint.Value.Device) as OsdPanelDevice;

					// Don't power off displays showing the osd
					if (osd != null)
						continue;
				}

				IDisplay display = Core.Originators.GetChild(destination.Endpoint.Device) as IDisplay;
				if (display != null)
					display.PowerOff();
			}

			// Power off the panels
			if (powerOff)
			{
				Originators.GetInstancesRecursive<IPanelDevice>()
				           .SelectMany(panel => panel.Controls.GetControls<IPowerDeviceControl>())
				           .ForEach(c => c.PowerOff());
			}
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
			settings.VolumePoint = m_VolumePoint;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_DialingPlan = default(DialingPlanInfo);

			m_VolumePoint = null;

			m_ConferenceManager.ClearDialingProviders();
			m_ConferenceManager.Favorites = null;
			m_ConferenceManager.DialingPlan.ClearMatchers();
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

			// Volume Point
			SetVolumePoint(settings.VolumePoint);

			// Favorites
			string path = PathUtils.GetProgramConfigPath("favorites");
			m_ConferenceManager.Favorites = new SqLiteFavorites(path);
		}

		/// <summary>
		/// Sets the volume point from the settings.
		/// </summary>
		/// <param name="volumePoint"></param>
		private void SetVolumePoint(VolumePoint volumePoint)
		{
			m_VolumePoint = volumePoint;
		}

		/// <summary>
		/// Sets the dialing plan from the settings.
		/// </summary>
		/// <param name="planInfo"></param>
		/// <param name="factory"></param>
		private void SetDialingPlan(DialingPlanInfo planInfo, IDeviceFactory factory)
		{
			m_DialingPlan = planInfo;

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

				m_DialingPlan = new DialingPlanInfo(video, audio);
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
				Logger.AddEntry(eSeverity.Error, "{0} failed add {1} dialing provider - {2}", this, sourceType, e.Message);
			}
		}

		#endregion
	}
}
