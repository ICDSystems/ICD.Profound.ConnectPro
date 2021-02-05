using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.LeftMenu
{
	[ViewBinding(typeof(IWtcLeftMenuView))]
	public sealed partial class WtcLeftMenuView : AbstractUiView, IWtcLeftMenuView
	{
		private readonly List<IWtcReferencedLeftMenuView> m_ChildViewList;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public WtcLeftMenuView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildViewList = new List<IWtcReferencedLeftMenuView>();
		}

		/// <summary>
		/// Returns child views for the left menu items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public IEnumerable<IWtcReferencedLeftMenuView> GetChildViews(IViewFactory factory, ushort count)
		{
			return GetChildViews(factory, m_ChildList, m_ChildViewList, count);
		}
	}
}
