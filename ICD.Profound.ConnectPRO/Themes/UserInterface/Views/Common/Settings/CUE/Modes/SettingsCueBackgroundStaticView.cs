using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.CUE.Modes;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.CUE.Modes
{
	[ViewBinding(typeof(ISettingsCueBackgroundStaticView))]
	public sealed partial class SettingsCueBackgroundStaticView : AbstractSettingsCueBackgroundModeView,
	                                                              ISettingsCueBackgroundStaticView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsCueBackgroundStaticView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
