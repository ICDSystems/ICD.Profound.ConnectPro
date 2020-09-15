using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.HeaderNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.HeaderNotifications
{
	[ViewBinding(typeof(IOsdUpcomingMeetingIndicatorView))]
	public sealed partial class OsdUpcomingMeetingIndicatorView : AbstractOsdView, IOsdUpcomingMeetingIndicatorView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public OsdUpcomingMeetingIndicatorView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
