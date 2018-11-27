using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Popups
{
	public sealed partial class OsdMuteView : AbstractOsdView, IOsdMuteView
	{
		public OsdMuteView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}
	}
}
