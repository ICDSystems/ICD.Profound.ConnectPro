using System;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.CUE.Modes;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings.CUE.Modes
{
	[ViewBinding(typeof(ISettingsCueBackgroundSeasonalView))]
	public sealed partial class SettingsCueBackgroundSeasonalView : AbstractSettingsCueBackgroundModeView,
	                                                                ISettingsCueBackgroundSeasonalView
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsCueBackgroundSeasonalView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
