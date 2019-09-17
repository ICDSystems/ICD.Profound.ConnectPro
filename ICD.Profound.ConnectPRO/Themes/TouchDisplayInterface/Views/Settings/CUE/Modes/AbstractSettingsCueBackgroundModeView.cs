using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.CUE.Modes;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings.CUE.Modes
{
	public abstract class AbstractSettingsCueBackgroundModeView : AbstractTouchDisplayView, ISettingsCueBackgroundModeView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractSettingsCueBackgroundModeView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
