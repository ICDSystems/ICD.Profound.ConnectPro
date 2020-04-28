using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Background;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Background
{
	[ViewBinding(typeof(IBackgroundView))]
	public sealed partial class BackgroundView : AbstractTouchDisplayView, IBackgroundView
	{
		public BackgroundView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetBackgroundMode(eTouchCueBackgroundMode mode)
		{
			m_Background.SetMode((ushort) mode);
		}

		public void SetBackgroundMotion(bool motion)
		{
			// just using "enabled" state as a bool
			m_Background.Enable(motion);
		}
	}
}