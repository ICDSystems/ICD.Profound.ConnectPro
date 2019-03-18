using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	[ViewBinding(typeof(IReferencedRouteListItemView))]
	public sealed partial class ReferencedRouteListItemView : AbstractUiView, IReferencedRouteListItemView
	{
		public ReferencedRouteListItemView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index) : base(panel, theme, parent, index)
		{
		}

		public void SetRoomLabelText(string room)
		{
			m_RoomLabel.SetLabelText(room);
		}

		public void SetDisplayLabelText(string display)
		{
			m_DisplayLabel.SetLabelText(display);
		}

		public void SetSourceLabelText(string source)
		{
			m_SourceLabel.SetLabelText(source);
		}
	}
}
