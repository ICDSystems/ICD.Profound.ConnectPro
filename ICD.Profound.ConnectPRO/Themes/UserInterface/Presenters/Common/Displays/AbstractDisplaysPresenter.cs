using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public abstract class AbstractDisplaysPresenter<TView> : AbstractUiPresenter<TView>, IDisplaysPresenter where TView : class, IUiView
	{
		public event MenuDestinationPressedCallback OnDestinationPressed;

		private ISource m_SelectedSource;

		protected abstract List<MenuDisplaysPresenterDisplay> Displays { get; }

		/// <summary>
		/// Gets/sets the source that is actively selected for routing.
		/// </summary>
		public ISource SelectedSource
		{
			get { return m_SelectedSource; }
			set
			{
				if (value == m_SelectedSource)
					return;

				m_SelectedSource = value;

				bool refresh = false;

				foreach (var display in Displays.ToArray())
					refresh |= display.SetSelectedSource(m_SelectedSource);

				if (refresh)
					RefreshIfVisible();
			}
		}

		protected AbstractDisplaysPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDestinationPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			bool combine = room != null && room.IsCombineRoom();
			IDestination[] destinations = room == null ? new IDestination[0] : room.Routing.Destinations.GetDisplayDestinations().ToArray();
			bool roomHasAudio = room != null && room.Routing.Destinations.RoomHasAudio;
			for (int i = 0; i < Displays.Count; i++)
			{
				var display = Displays[i];
				display.SetRoomCombine(combine);
				display.SetRoomHasAudio(roomHasAudio);
				IDestination dest;
				destinations.TryElementAt(i, out dest);
				display.SetDestination(dest);
			}

			base.SetRoom(room);
		}

		public void SetRouting(IDictionary<IDestination, IcdHashSet<ISource>> routing, IcdHashSet<ISource> activeAudio)
		{
			bool refresh = false;

			foreach (var display in Displays.ToArray())
			{
				refresh |= display.SetRoutedSource(GetRoutedSource(display.Destination, routing));
				refresh |= display.SetAudioActive(display.RoutedSource != null && activeAudio.Contains(display.RoutedSource));
			}

			if (refresh)
				RefreshIfVisible();
		}

		protected void DisplayButtonPressed(MenuDisplaysPresenterDisplay display)
		{
			MenuDestinationPressedCallback handler = OnDestinationPressed;
			if (handler != null)
				handler(this, display.RoutedSource, display.Destination);
		}

		protected void DisplaySpeakerButtonPressed(MenuDisplaysPresenterDisplay display)
		{
			if (Room == null)
				return;

			if (display.RoutedSource == null || !display.RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio))
				return;

			Room.Routing.RouteAudio(display.RoutedSource);
		}

		private static ISource GetRoutedSource(IDestination destination, IDictionary<IDestination, IcdHashSet<ISource>> routing)
		{
			if (destination == null)
				return null;

			IcdHashSet<ISource> sources;
			if (!routing.TryGetValue(destination, out sources))
				return null;

			return sources.OrderBy(s => s.Order)
			              .ThenBy(s => s.Id)
			              .FirstOrDefault();
		}
	}
}
