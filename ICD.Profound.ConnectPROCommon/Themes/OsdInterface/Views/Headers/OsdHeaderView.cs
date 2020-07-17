using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Headers
{
	[ViewBinding(typeof(IOsdHeaderView))]
	public sealed partial class OsdHeaderView : AbstractOsdView, IOsdHeaderView
	{
		public OsdHeaderView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetRoomName(string name)
		{
			m_RoomName.SetLabelText(name);
		}

		public void SetTimeLabel(string time)
		{
			m_TimeLabel.SetLabelText(time);
		}

		/// <summary>
		/// Sets the current touch-free face graphic.
		/// </summary>
		/// <param name="image"></param>
		public void SetTouchFreeFaceImage(eTouchFreeFace image)
		{
			m_TouchFreeFace.SetMode((ushort)image);
		}
	}
}
