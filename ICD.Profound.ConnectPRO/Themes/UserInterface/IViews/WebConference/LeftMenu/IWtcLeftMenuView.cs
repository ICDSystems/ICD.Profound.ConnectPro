using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu
{
	public interface IWtcLeftMenuView : IUiView
	{
		/// <summary>
		/// Returns child views for the left menu items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IWtcReferencedLeftMenuView> GetChildViews(IViewFactory factory, ushort count);
	}
}