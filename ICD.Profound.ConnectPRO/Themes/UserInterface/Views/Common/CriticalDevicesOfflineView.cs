using System.Linq;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	[ViewBinding(typeof(ICriticalDevicesOfflineView))]
	public sealed partial class CriticalDevicesOfflineView : AbstractUiView, ICriticalDevicesOfflineView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public CriticalDevicesOfflineView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetCriticalDevicesOffline(string text)
		{
			m_Devices.SetLabelTextAtJoin(m_Devices.SerialLabelJoins.First(), text);
		}
	}
}