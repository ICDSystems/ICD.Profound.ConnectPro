﻿using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.CUE.Modes;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings.CUE.Modes
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
		public SettingsCueBackgroundStaticView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
