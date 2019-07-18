using ICD.Connect.UI.Mvp.Views;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays
{
	public interface IMenu3PlusDisplaysView : IUiView
	{
		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedDisplayView> GetChildComponentViews(IViewFactory factory, ushort count);

		/// <summary>
		/// Scrolls back to the first item in the list.
		/// </summary>
		void ResetScrollPosition();
	}
}
