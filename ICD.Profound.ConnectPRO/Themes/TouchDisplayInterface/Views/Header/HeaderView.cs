using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Header
{
	[ViewBinding(typeof(ITouchDisplayHeaderView))]
	public sealed partial class TouchDisplayHeaderView : AbstractTouchDisplayView, ITouchDisplayHeaderView
	{
		public TouchDisplayHeaderView(ISigInputOutput panel, ConnectProTheme theme)
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
