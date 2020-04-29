using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Popups;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Popups
{
	[ViewBinding(typeof(IOsdMuteView))]
	public sealed partial class OsdMuteView : AbstractOsdView, IOsdMuteView
	{
		public OsdMuteView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
