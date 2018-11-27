using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	public sealed partial class OsdHeaderView : AbstractOsdView, IOsdHeaderView
	{
		public void SetRoomName(string name)
		{
			m_RoomName.SetLabelText(name);
		}
	}
}
