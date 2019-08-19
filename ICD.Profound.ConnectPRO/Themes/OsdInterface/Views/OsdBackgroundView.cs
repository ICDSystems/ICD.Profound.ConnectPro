using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	[ViewBinding(typeof(IOsdBackgroundView))]
	public sealed partial class OsdBackgroundView : AbstractOsdView, IOsdBackgroundView
	{
		public OsdBackgroundView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetBackgroundMode(eCueBackgroundMode mode)
		{
			m_Background.SetMode((ushort)mode);
		}
	}
}
