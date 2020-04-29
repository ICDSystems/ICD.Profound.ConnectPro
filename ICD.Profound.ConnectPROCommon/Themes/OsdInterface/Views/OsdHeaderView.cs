using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views
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
	}
}
