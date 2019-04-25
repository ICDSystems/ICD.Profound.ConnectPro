using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	[PresenterBinding(typeof(IMenuRouteSummaryPresenter))]
	public sealed class MenuRouteSummaryPresenter : AbstractUiPresenter<IMenuRouteSummaryView>, IMenuRouteSummaryPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedRouteListItemPresenterFactory m_PresenterFactory;

		public MenuRouteSummaryPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new ReferencedRouteListItemPresenterFactory(nav, ItemFactory, (a) => { }, (a) => { });
		}

		protected override void Refresh(IMenuRouteSummaryView view)
		{
			base.Refresh(view);
			
			if (Room == null)
				return;

			m_RefreshSection.Enter();
			try
			{
				foreach (var presenter in m_PresenterFactory)
					presenter.Refresh();
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void SetRouting(IDictionary<IDestination, IcdHashSet<ISource>> routing)
		{
			IEnumerable<RouteListItem> models = routing.SelectMany(d => d.Value.Select(s => GetListItem(d.Key, s)));
			m_PresenterFactory.BuildChildren(models);
			RefreshIfVisible();
		}

		private RouteListItem GetListItem(IDestination destination, ISource source)
		{
			IRoom room = Room.GetRoomsRecursive().FirstOrDefault(r => r.Originators.Contains(destination.Id));
			return new RouteListItem(room, destination, source);
		}

		private IEnumerable<IReferencedRouteListItemView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#region View Callbacks

		protected override void Subscribe(IMenuRouteSummaryView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
		}

		protected override void Unsubscribe(IMenuRouteSummaryView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
		}

		private void ViewOnCloseButtonPressed(object sender, System.EventArgs e)
		{
			ShowView(false);
		}

		#endregion
	}
}
