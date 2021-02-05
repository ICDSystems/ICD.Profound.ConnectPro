using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Indicator;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Indicator
{
	[ViewBinding(typeof(IUpcomingMeetingIndicatorView))]
	public sealed partial class UpcomingMeetingIndicatorView : AbstractUiView, IUpcomingMeetingIndicatorView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public UpcomingMeetingIndicatorView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void PlaySound(bool playing)
		{
			if (playing)
				m_Sound.Play();
			else
				m_Sound.Stop();
		}
	}
}