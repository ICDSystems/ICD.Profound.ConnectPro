using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Headers
{
	public sealed partial class OsdCriticalDevicesOfflineView 
	{
		private VtProSubpage m_Subpage;
		private VtProSubpageReferenceList m_DeviceList;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 900
			};
			m_DeviceList = new VtProSubpageReferenceList(9, panel as IPanelDevice, m_Subpage)
			{
				MaxSize = 6,
				SerialJoinIncrement = 2
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_DeviceList;
		}
	}
}