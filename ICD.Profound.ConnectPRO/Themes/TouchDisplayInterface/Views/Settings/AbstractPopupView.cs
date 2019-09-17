using System;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings
{
	public abstract class AbstractPopupView : AbstractTouchDisplayView, IPopupView
	{
		public abstract event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractPopupView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
