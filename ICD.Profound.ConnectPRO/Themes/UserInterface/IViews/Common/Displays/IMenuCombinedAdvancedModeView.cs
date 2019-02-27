using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays
{
	public interface IMenuCombinedAdvancedModeView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the Simple Mode button.
		/// </summary>
		event EventHandler OnSimpleModeButtonPressed;

		/// <summary>
		/// Raised when the user presses the Route Summary button.
		/// </summary>
		event EventHandler OnRouteSummaryButtonPressed;

		/// <summary>
		/// Sets the enabled state of the Simple Mode button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetSimpleModeButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the enabled state of the Route Summary button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetRouteSummaryButtonEnabled(bool enabled);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedAdvancedDisplayView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
