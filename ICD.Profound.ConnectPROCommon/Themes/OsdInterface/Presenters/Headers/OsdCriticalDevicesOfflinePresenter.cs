using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Rooms;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Headers;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Headers
{
	[PresenterBinding(typeof(IOsdCriticalDevicesOfflinePresenter))]
	public sealed class OsdCriticalDevicesOfflinePresenter : AbstractOsdPresenter<IOsdCriticalDevicesOfflineView>,
	                                                         IOsdCriticalDevicesOfflinePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly IcdHashSet<IDevice> m_SubscribedDevices;
		private readonly List<IDevice> m_OfflineDevices;
		private readonly ReferencedCriticalDevicePresenterFactory m_ChildrenFactory;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public OsdCriticalDevicesOfflinePresenter(IOsdNavigationController nav, IOsdViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_SubscribedDevices = new IcdHashSet<IDevice>();
			m_OfflineDevices = new List<IDevice>();
			m_ChildrenFactory = new ReferencedCriticalDevicePresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
		}

		protected override void Refresh(IOsdCriticalDevicesOfflineView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				foreach (IReferencedCriticalDevicePresenter presenter in
					m_ChildrenFactory.BuildChildren(m_OfflineDevices))
					presenter.ShowView(true);
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

		private IEnumerable<IReferencedCriticalDeviceView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory as IOsdViewFactory, count);
		}

		private void UpdateDevices()
		{
			IEnumerable<IDevice> devices =
				Room == null
					? Enumerable.Empty<IDevice>()
					: Room.GetCriticalDevicesRecursive();

			m_SubscribedDevices.ForEach(Unsubscribe);
			m_SubscribedDevices.SetRange(devices);
			m_SubscribedDevices.ForEach(Subscribe);

			UpdateOfflineDevices();
		}

		private void UpdateOfflineDevices()
		{
			IEnumerable<IDevice> offline =
				Room == null
					? Enumerable.Empty<IDevice>()
					: Room.GetOfflineCriticalDevicesRecursive();

			m_OfflineDevices.SetRange(offline);

			ShowView(m_OfflineDevices.Count > 0);

			RefreshIfVisible();
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
		}

		private void Unsubscribe(IDevice device)
		{
			device.OnIsOnlineStateChanged -= DeviceOnIsOnlineStateChanged;
		}

		private void DeviceOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs eventArgs)
		{
			UpdateOfflineDevices();
		}

		#endregion

		#region Child Callbacks

		private void Subscribe(IReferencedCriticalDevicePresenter presenter)
		{
		}

		private void Unsubscribe(IReferencedCriticalDevicePresenter presenter)
		{
		}

		#endregion
	}
}