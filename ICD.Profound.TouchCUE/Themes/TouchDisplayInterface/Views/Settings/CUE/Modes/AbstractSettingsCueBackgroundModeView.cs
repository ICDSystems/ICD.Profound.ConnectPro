using ICD.Connect.Panels;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Settings.CUE.Modes;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Settings.CUE.Modes
{
	public abstract class AbstractSettingsCueBackgroundModeView : AbstractTouchDisplayView, ISettingsCueBackgroundModeView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractSettingsCueBackgroundModeView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
		}
	}
}
