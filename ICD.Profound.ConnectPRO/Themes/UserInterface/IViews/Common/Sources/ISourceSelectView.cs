using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources
{
	public interface ISourceSelectView : IView
	{
		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedSourceSelectView> GetChildComponentViews(IViewFactory factory, ushort count);

		/// <summary>
		/// Sets the number of sources that are available.
		/// </summary>
		/// <param name="count"></param>
		void SetSourceCount(ushort count);

		/// <summary>
		/// Sets the number of displays that are available.
		/// </summary>
		/// <param name="count"></param>
		void SetDisplayCount(ushort count);
	}
}
