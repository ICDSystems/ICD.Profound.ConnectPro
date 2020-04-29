using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference
{
	[ViewBinding(typeof(IPrivacyMuteConferenceView))]
	public sealed partial class PrivacyMuteConferenceView : AbstractTouchDisplayView, IPrivacyMuteConferenceView
	{
		public PrivacyMuteConferenceView(ISigInputOutput panel, TouchCueTheme theme) : base(panel, theme)
		{
		}
	}
}
