using System;
using System.Collections.Generic;
using System.Text;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters
{
	public static class TouchCueIcons
	{
		public static string GetIcon(string iconName)
		{
			return GetIcon(iconName, eTouchCueColor.White);
		}

		public static string GetIcon(string iconName, eTouchCueColor color)
		{
			return string.Format("ic_{0}_{1}", iconName.ToLower(), color.ToString().ToLower());
		}
	}

	public enum eTouchCueColor
	{
		White,
		Gray,
		LightBlue,
		Red,
		Green
	}
}
