using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference
{
	[ViewBinding(typeof(IPrivacyMuteConferenceView))]
	public sealed partial class PrivacyMuteConferenceView : AbstractTouchDisplayView, IPrivacyMuteConferenceView
	{
		public PrivacyMuteConferenceView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}
	}
}
