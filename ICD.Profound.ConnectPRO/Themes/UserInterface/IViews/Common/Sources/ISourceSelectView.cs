using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Sources
{
	public interface ISourceSelectView : IUiView
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

		/// <summary>
		/// Sets the combined state of the room.
		/// </summary>
		void SetCombined(bool combined);

		/// <summary>
		/// Scrolls back to the first item in the list.
		/// </summary>
		void ResetScrollPosition();
	}
}
