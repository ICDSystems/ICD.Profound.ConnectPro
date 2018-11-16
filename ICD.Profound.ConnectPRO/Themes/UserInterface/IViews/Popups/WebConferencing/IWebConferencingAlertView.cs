using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing
{
	public interface IWebConferencingAlertView : IUiView
	{
		event EventHandler OnDismissButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedWebConferencingAlertView> GetChildComponentViews(IViewFactory factory, ushort count);

		void SetAppCount(ushort count);
	}
}
