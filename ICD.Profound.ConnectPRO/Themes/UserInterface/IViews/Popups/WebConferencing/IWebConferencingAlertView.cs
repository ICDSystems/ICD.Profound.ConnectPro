using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing
{
	public interface IWebConferencingAlertView : IView
	{
		event EventHandler OnDismissButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedWebConferencingAlertView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
