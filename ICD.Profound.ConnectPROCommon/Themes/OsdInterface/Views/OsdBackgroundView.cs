using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views
{
	[ViewBinding(typeof(IOsdBackgroundView))]
	public sealed partial class OsdBackgroundView : AbstractOsdView, IOsdBackgroundView
	{
		public OsdBackgroundView(ISigInputOutput panel, IConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetBackgroundMode(eCueBackgroundMode mode)
		{
			m_Background.SetMode((ushort)mode);
		}
	}
}
