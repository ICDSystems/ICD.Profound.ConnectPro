using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	[ViewBinding(typeof(IHeaderView))]
	public sealed partial class HeaderView : AbstractUiView, IHeaderView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public HeaderView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Sets the visibility of the view.
		/// </summary>
		/// <param name="visible"></param>
		public override void Show(bool visible)
		{
			// Always visible
		}

		/// <summary>
		/// Sets the room name label text.
		/// </summary>
		/// <param name="name"></param>
		public void SetRoomName(string name)
		{
			m_RoomNameLabel.SetLabelText(name);
		}

		/// <summary>
		/// Sets the time label text.
		/// </summary>
		/// <param name="label"></param>
		public void SetTimeLabel(string label)
		{
			m_TimeLabel.SetLabelText(label);
		}
	}
}
