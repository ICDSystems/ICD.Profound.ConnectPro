using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	public sealed partial class OsdSourcesView : AbstractOsdView, IOsdSourcesView
	{
		public OsdSourcesView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetRoomName(string name)
		{
			m_RoomLabel.SetLabelText(name);
		}

		public void SetIcon(ushort index, string icon)
		{
			m_Icons[index].SetIcon(icon);
		}

		public void SetLabel(ushort index, string label)
		{
			m_Labels[index].SetLabelText(label);
		}
	}
}
