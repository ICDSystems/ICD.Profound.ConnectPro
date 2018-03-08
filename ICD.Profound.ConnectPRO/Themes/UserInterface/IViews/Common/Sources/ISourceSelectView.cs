using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources
{
	public interface ISourceSelectView : IView
	{
		/// <summary>
		/// Sets the visibility of the arrows indicating there are more than 4 items.
		/// </summary>
		/// <param name="show"></param>
		void ShowArrows(bool show);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedSourceSelectView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
