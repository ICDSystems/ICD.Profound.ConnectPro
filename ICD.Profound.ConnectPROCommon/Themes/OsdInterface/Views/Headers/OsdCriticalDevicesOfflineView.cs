using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Headers
{
	[ViewBinding(typeof(IOsdCriticalDevicesOfflineView))]
	public sealed partial class OsdCriticalDevicesOfflineView : AbstractOsdView, IOsdCriticalDevicesOfflineView
	{
		private readonly List<IReferencedCriticalDeviceView> m_ChildList;
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public OsdCriticalDevicesOfflineView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildList = new List<IReferencedCriticalDeviceView>();
		}

		/*public void SetCriticalDevicesOffline(string text)
		{
			m_DeviceList.SetLabelText(text);
		}*/

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IReferencedCriticalDeviceView> GetChildComponentViews(IOsdViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_DeviceList, m_ChildList, count);
		}
	}
}