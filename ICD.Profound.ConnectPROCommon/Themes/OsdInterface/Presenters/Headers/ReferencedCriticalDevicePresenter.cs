using System.Linq;
using ICD.Common.Logging.Activities;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Headers;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Headers
{
	[PresenterBinding(typeof(IReferencedCriticalDevicePresenter))]
	public sealed class ReferencedCriticalDevicePresenter :
		AbstractOsdComponentPresenter<IReferencedCriticalDeviceView>, IReferencedCriticalDevicePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private IDevice m_Device;

		[CanBeNull]
		public IDevice Device
		{
			get { return m_Device; }
			set
			{
				if (value == m_Device)
					return;

				Unsubscribe(m_Device);
				m_Device = value;
				Subscribe(m_Device);

				RefreshIfVisible();
			}
		}

		public ReferencedCriticalDevicePresenter(IOsdNavigationController nav, IOsdViewFactory views,
		                                         IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IReferencedCriticalDeviceView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string label = GetErrorMessage(Device);

				string icon = GetDeviceIcon(Device);
				
				view.SetSubjectIcon(icon);
				view.SetSubjectLabel(label);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private string GetDeviceIcon([CanBeNull] IDevice device)
		{
			if (Room == null || device == null)
				return string.Empty;

			return Room.Routing
			           .Sources
			           .GetRoomSources()
			           .OfType<ConnectProSource>()
			           .Where(s => s.Id == device.Id)
			           .Select(s => s.Icon)
			           .FirstOrDefault() ?? "settings";
		}
		

		private static string GetErrorMessage([CanBeNull] IDevice device)
		{
			if (device == null)
				return string.Empty;

			return
				device.Activities
				      .Where(a => a.Severity <= eSeverity.Error)
				      .OrderBy(a => a.Priority)
				      .ThenBy(a => a.Message)
				      // Hack - Offline activity may not have been logged yet
				      .Append(DeviceBaseActivities.GetIsOnlineActivity(false))
				      .Select(a => string.Format("{0}: {1} - {2}", device.Name, a.Severity, a.Message))
				      .First();
		}

		private void Subscribe(IDevice device)
		{
			if (device == null)
				return;

			device.Activities.OnActivityChanged += DeviceActivitiesOnActivityChanged;
		}

		private void Unsubscribe(IDevice device)
		{
			if (device == null)
				return;

			device.Activities.OnActivityChanged -= DeviceActivitiesOnActivityChanged;
		}

		private void DeviceActivitiesOnActivityChanged(object sender, GenericEventArgs<Activity> e)
		{
			RefreshIfVisible();
		}
	}
}
