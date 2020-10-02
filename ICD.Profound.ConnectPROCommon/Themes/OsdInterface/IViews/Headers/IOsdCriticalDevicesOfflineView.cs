using System.Collections.Generic;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers
{
	public interface IOsdCriticalDevicesOfflineView :IOsdView
	{
		//void SetCriticalDevicesOffline(string text);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedCriticalDeviceView> GetChildComponentViews(IOsdViewFactory factory, ushort count);
	}
}