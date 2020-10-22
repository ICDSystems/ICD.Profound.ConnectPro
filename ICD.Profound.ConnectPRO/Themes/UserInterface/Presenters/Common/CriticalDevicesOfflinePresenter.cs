using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Logging.Activities;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Rooms;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common
{
	[PresenterBinding(typeof(ICriticalDevicesOfflinePresenter))]
	public sealed class CriticalDevicesOfflinePresenter : AbstractUiPresenter<ICriticalDevicesOfflineView>,
	                                                             ICriticalDevicesOfflinePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly IcdHashSet<IDevice> m_CriticalDevices;
		private bool m_CriticalDeviceOffline;

		private bool CriticalDeviceOffline
		{
			get { return m_CriticalDeviceOffline; }
			set
			{
				if (value == m_CriticalDeviceOffline)
					return;

				m_CriticalDeviceOffline = value;

				RefreshIfVisible();
				ShowView(m_CriticalDeviceOffline);
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CriticalDevicesOfflinePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_CriticalDevices = new IcdHashSet<IDevice>();
		}

		protected override void Refresh(ICriticalDevicesOfflineView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<string> lines =
					Room == null
						? Enumerable.Empty<string>()
						: Room.GetOfflineCriticalDevicesRecursive()
						      .SelectMany(d => GetErrorMessageLines(d));

				string text = string.Join("\n", lines.ToArray());

				view.SetCriticalDevicesOffline(text);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			UpdateDevices();
		}

		#region Private Methods

		private void UpdateDevices()
		{
			IEnumerable<IDevice> devices =
				Room == null
					? Enumerable.Empty<IDevice>()
					: Room.GetCriticalDevicesRecursive();

			m_CriticalDevices.ForEach(Unsubscribe);
			m_CriticalDevices.SetRange(devices);
			m_CriticalDevices.ForEach(Subscribe);

			UpdateCriticalDevicesOffline();
		}

		private void UpdateCriticalDevicesOffline()
		{
			CriticalDeviceOffline = Room != null && Room.GetOfflineCriticalDevicesRecursive().Any();

			bool isVisible = Room != null && Room.GetOfflineCriticalDevicesRecursive().Any();
			ShowView(isVisible);

			RefreshIfVisible();
		}

		private IEnumerable<string> GetErrorMessageLines(IDevice device)
		{
			foreach (Activity activity in device.Activities.Where(a => a.Severity <= eSeverity.Error))
				yield return string.Format("{0}   {1} - {2}", device.Name, activity.Severity, activity.Message);

			yield return string.Empty;
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Subscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.Originators.OnCollectionChanged += OriginatorsOnCollectionChanged;
		}

		/// <summary>
		/// Unsubscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		protected override void Unsubscribe(IConnectProRoom room)
		{
			base.Subscribe(room);

			if (room == null)
				return;

			room.Originators.OnCollectionChanged -= OriginatorsOnCollectionChanged;
		}

		private void OriginatorsOnCollectionChanged(object sender, EventArgs eventArgs)
		{
			UpdateDevices();
		}

		#endregion

		#region Device Callbacks

		private void Subscribe(IDevice device)
		{
			device.OnIsOnlineStateChanged += DeviceOnIsOnlineStateChanged;
			device.Activities.OnActivityChanged += DeviceActivitiesOnActivityChanged;
		}

		private void Unsubscribe(IDevice device)
		{
			device.OnIsOnlineStateChanged -= DeviceOnIsOnlineStateChanged;
			device.Activities.OnActivityChanged -= DeviceActivitiesOnActivityChanged;
		}

		private void DeviceOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs eventArgs)
		{
			UpdateCriticalDevicesOffline();
		}

		private void DeviceActivitiesOnActivityChanged(object sender, GenericEventArgs<Activity> e)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}