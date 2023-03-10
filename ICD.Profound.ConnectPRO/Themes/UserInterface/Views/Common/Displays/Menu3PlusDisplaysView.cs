using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	[ViewBinding(typeof(IMenu3PlusDisplaysView))]
	public sealed partial class Menu3PlusDisplaysView : AbstractUiView, IMenu3PlusDisplaysView
	{
		private readonly List<IReferencedDisplayView> m_ChildViews;

		public Menu3PlusDisplaysView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
			m_ChildViews = new List<IReferencedDisplayView>();
		}

		public IEnumerable<IReferencedDisplayView> GetChildComponentViews(IViewFactory factory, ushort count)
		{
			return factory.LazyLoadSrlViews(m_DisplayList, m_ChildViews, count);
		}

		public void ResetScrollPosition()
		{
			m_DisplayList.ScrollToItem(0);
		}
	}
}
