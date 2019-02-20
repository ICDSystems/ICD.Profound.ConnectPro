using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Popups
{
	[ViewBinding(typeof(IOsdMuteView))]
	public sealed partial class OsdMuteView : AbstractOsdView, IOsdMuteView
	{
		public OsdMuteView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
