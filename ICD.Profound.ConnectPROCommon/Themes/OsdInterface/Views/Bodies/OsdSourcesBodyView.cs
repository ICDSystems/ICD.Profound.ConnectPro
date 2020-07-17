using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Bodies
{
	[ViewBinding(typeof(IOsdSourcesBodyView))]
	public sealed partial class OsdSourcesBodyView : AbstractOsdView, IOsdSourcesBodyView
	{
		public OsdSourcesBodyView(ISigInputOutput panel, IConnectProTheme theme)
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
