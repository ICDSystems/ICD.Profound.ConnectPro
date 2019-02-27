using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays
{
	public interface IMenuRouteSummaryView : IUiView
	{
		/// <summary>
		/// Raised when the user presses the Close button.
		/// </summary>
		event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedRouteListItemView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
