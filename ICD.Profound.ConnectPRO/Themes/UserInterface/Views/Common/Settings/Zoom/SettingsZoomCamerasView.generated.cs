using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Lists;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Zoom
{
	public sealed partial class SettingsZoomCamerasView
	{
		private VtProDynamicButtonList m_CameraDeviceButtonList;
		private VtProDynamicButtonList m_UsbIdButtonList;
		private VtProSubpage m_Subpage;

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
				DigitalVisibilityJoin = 953
			};

            m_CameraDeviceButtonList = new VtProDynamicButtonList(18, panel as IPanelDevice, m_Subpage)
            {
				MaxSize = 10
            };

			m_UsbIdButtonList = new VtProDynamicButtonList(17, panel as IPanelDevice, m_Subpage)
			{
				DigitalEnableJoin = 960,
				MaxSize = 10
			};
        }

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
        {
            yield return m_Subpage;
            yield return m_CameraDeviceButtonList;
            yield return m_UsbIdButtonList;
        }
	}
}
