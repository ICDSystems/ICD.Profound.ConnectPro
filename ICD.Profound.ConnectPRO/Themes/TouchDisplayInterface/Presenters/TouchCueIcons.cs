using System;
using System.Collections.Generic;
using System.Text;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters
{
	public static class TouchCueIcons
	{
		public static string GetIcon(string iconName)
		{
			return string.Format("ic_{0}", iconName.ToLower());
		}
	}
}
