using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Notifications
{
	[ViewBinding(typeof(IConferenceConnectingView))]
	public sealed partial class ConferenceConnectingView : AbstractTouchDisplayView, IConferenceConnectingView
	{
		public ConferenceConnectingView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetNotificationText(string text)
		{
			m_NotificationText.SetLabelText(text);
		}
	}
}
