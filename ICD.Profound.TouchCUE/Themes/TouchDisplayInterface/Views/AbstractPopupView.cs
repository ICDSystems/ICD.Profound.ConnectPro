using System;
using ICD.Connect.Panels;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views
{
	public abstract class AbstractPopupView : AbstractTouchDisplayView, IPopupView
	{
		public abstract event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractPopupView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
		}
	}
}
