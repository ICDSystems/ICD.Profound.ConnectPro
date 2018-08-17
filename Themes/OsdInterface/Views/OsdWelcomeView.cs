using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
    public sealed partial class OsdWelcomeView : AbstractOsdView, IOsdWelcomeView
    {
		public OsdWelcomeView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetRoomName(string name)
		{
			m_RoomLabel.SetLabelText(name);
		}
	}
}
