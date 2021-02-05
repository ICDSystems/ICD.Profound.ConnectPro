using System;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups
{
	public abstract class AbstractPopupView : AbstractUiView, IPopupView
	{
		public abstract event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractPopupView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}
	}
}
