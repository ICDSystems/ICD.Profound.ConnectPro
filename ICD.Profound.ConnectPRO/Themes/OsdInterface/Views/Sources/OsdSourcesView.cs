using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Sources;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Sources
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
			m_NameLabels[index].SetLabelText(label);
		}

		public void SetDescription(ushort index, string description)
		{
			m_DescriptionLabels[index].SetLabelText(description);
		}
	}
}
