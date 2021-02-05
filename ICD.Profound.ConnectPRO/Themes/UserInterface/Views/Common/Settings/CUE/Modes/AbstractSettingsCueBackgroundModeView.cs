using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.CUE.Modes;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.CUE.Modes
{
	public abstract class AbstractSettingsCueBackgroundModeView : AbstractUiView, ISettingsCueBackgroundModeView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractSettingsCueBackgroundModeView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
