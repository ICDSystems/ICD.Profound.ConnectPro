using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference
{
	[ViewBinding(typeof(IShareConferenceView))]
	public sealed partial class ShareConferenceView : AbstractTouchDisplayView, IShareConferenceView
	{
		public ShareConferenceView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}
	}
}
