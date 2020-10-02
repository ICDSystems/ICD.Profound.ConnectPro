using ICD.Common.Properties;
using ICD.Connect.Devices;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Headers
{
	public interface IReferencedCriticalDevicePresenter : IOsdPresenter<IReferencedCriticalDeviceView>
	{
		[CanBeNull]
		IDevice Device { get; set; }
	}
}
