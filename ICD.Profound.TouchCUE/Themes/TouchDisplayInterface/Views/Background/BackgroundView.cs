using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Background;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Background
{
	[ViewBinding(typeof(IBackgroundView))]
	public sealed partial class BackgroundView : AbstractTouchDisplayView, IBackgroundView
	{
		public BackgroundView(ISigInputOutput panel, TouchCueTheme theme) : base(panel, theme)
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