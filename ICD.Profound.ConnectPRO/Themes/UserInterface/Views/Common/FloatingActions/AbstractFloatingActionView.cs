using System;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.FloatingActions;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.FloatingActions
{
	public abstract class AbstractFloatingActionView : AbstractUiView, IFloatingActionView
	{
		/// <summary>
		/// Raised when the user presses the option button.
		/// </summary>
		public abstract event EventHandler OnButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		protected AbstractFloatingActionView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Sets the selected state of the option.
		/// </summary>
		/// <param name="active"></param>
		public abstract void SetActive(bool active);

		/// <summary>
		/// Sets the enabled state of the option.
		/// </summary>
		/// <param name="enabled"></param>
		public abstract void SetEnabled(bool enabled);
	}
}
