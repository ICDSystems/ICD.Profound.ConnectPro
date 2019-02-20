using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	[ViewBinding(typeof(IOsdHeaderView))]
	public sealed partial class OsdHeaderView : AbstractOsdView, IOsdHeaderView
	{
		public OsdHeaderView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetRoomName(string name)
		{
			m_RoomName.SetLabelText(name);
		}
	}
}
