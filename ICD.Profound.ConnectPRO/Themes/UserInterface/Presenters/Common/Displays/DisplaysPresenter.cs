using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class DisplaysPresenter : AbstractPresenter<IDisplaysView>, IDisplaysPresenter
	{
		private readonly ReferencedDisplaysPresenterFactory m_ChildrenFactory;
		private readonly SafeCriticalSection m_RefreshSection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public DisplaysPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedDisplaysPresenterFactory(nav, ItemFactory);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		protected override void Refresh(IDisplaysView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<IDestination> destinations =
					Room == null
						? Enumerable.Empty<IDestination>()
						: Room.Routing.GetDisplayDestinations();

				foreach (IReferencedDisplaysPresenter presenter in m_ChildrenFactory.BuildChildren(destinations))
					presenter.ShowView(true);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<IReferencedDisplaysView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}
	}
}
