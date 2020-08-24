using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Indicator;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Indicator
{
	[ViewBinding(typeof(IUpcomingMeetingIndicatorView))]
	public sealed partial class UpcomingMeetingIndicatorView : AbstractUiView, IUpcomingMeetingIndicatorView
	{
		public void PlaySound(bool playing)
		{
			if (playing)
				m_Sound.Play(3000);
			else
				m_Sound.Stop();
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public UpcomingMeetingIndicatorView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}
	}
}