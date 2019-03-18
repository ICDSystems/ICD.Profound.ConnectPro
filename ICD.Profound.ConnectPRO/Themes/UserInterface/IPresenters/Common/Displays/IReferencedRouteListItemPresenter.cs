using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays
{
	public interface IReferencedRouteListItemPresenter : IUiPresenter<IReferencedRouteListItemView>
	{
		RouteListItem Model { set; }
	}

	public struct RouteListItem
	{
		private readonly IRoom m_Room;
		private readonly IDestination m_Destination;
		private readonly ISource m_Source;

		public IRoom Room { get { return m_Room; } }
		public IDestination Destination { get { return m_Destination; } }
		public ISource Source { get { return m_Source; } }

		public RouteListItem(IRoom room, IDestination destination, ISource source)
		{
			m_Room = room;
			m_Destination = destination;
			m_Source = source;
		}
	}
}
