using System;
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
	public abstract class AbstractDisplaysPresenter<TView> : AbstractUiPresenter<TView>, IDisplaysPresenter
		where TView : class, IUiView
	{
		protected const long DISPLAY_GAUGE_REFRESH_INTERVAL = 100;

		/// <summary>
		/// Raised when a display destination is pressed.
		/// </summary>
		public event MenuDestinationPressedCallback OnDestinationPressed;

		private ISource m_SelectedSource;

		private readonly List<MenuDisplaysPresenterDisplay> m_Displays;

		protected List<MenuDisplaysPresenterDisplay> Displays { get { return m_Displays; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractDisplaysPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_Displays = new List<MenuDisplaysPresenterDisplay>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDestinationPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			bool combine = room != null && room.IsCombineRoom();

			// Remake displays list
			m_Displays.ForEach(Unsubscribe);
			m_Displays.ForEach(d => d.Dispose());
			m_Displays.Clear();
			if (room != null)
				m_Displays.AddRange(room.Routing.Destinations.GetVideoDestinations()
				                        .Select(d => new MenuDisplaysPresenterDisplay(room, d,combine)));

			m_Displays.ForEach(Subscribe);

			base.SetRoom(room);
		}

		public void SetRouting(IDictionary<IDestinationBase, IcdHashSet<ISource>> routing, IcdHashSet<ISource> activeAudio)
		{
			bool refresh = false;

			foreach (MenuDisplaysPresenterDisplay display in Displays.ToArray())
			{
				ISource routedSource = GetRoutedSource(display.Destination, routing);
				bool canRouteToRoomAudio = routedSource != null &&
				                           display.Destination != null &&
				                           Room != null &&
				                           Room.Routing.HasPathToRoomAudio(routedSource);

				refresh |= display.SetRoutedSource(routedSource, canRouteToRoomAudio);
				refresh |= display.SetAudioActive(canRouteToRoomAudio && activeAudio.Contains(routedSource));
			}

			if (refresh)
				RefreshIfVisible();
		}

		/// <summary>
		/// Sets the source that is actively selected for routing.
		/// </summary>
		public void SetSelectedSource(ISource source)
		{
			if (source == m_SelectedSource)
				return;

			m_SelectedSource = source;

			bool refresh = false;

			foreach (MenuDisplaysPresenterDisplay display in Displays.ToArray())
			{
				bool canRouteVideo = false;

				if (Room != null && m_SelectedSource != null)
					canRouteVideo = Room.Routing.HasPath(m_SelectedSource, display.Destination, eConnectionType.Video);

				refresh |= display.SetSelectedSource(m_SelectedSource, canRouteVideo);
			}

			if (refresh)
				RefreshIfVisible();
		}

		#endregion

		#region Private Methods

		protected void DisplayButtonPressed(MenuDisplaysPresenterDisplay display)
		{
			// Ignore presses for offline displays
			if (!display.Enabled)
				return;

			MenuDestinationPressedCallback handler = OnDestinationPressed;
			if (handler != null)
				handler(this, display.RoutedSource, display.Destination);
		}

		protected void DisplaySpeakerButtonPressed(MenuDisplaysPresenterDisplay display)
		{
			// Ignore presses for offline displays
			if (!display.Enabled)
				return;

			if (Room == null)
				return;

			if (display.RoutedSource == null || !display.RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio))
				return;

			Room.Routing.RouteToRoomAudio(display.RoutedSource);
		}

		private static ISource GetRoutedSource(IDestinationBase destination, IDictionary<IDestinationBase, IcdHashSet<ISource>> routing)
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

		#endregion

		#region Display Callbacks

		/// <summary>
		/// Subscribe to the display events.
		/// </summary>
		/// <param name="display"></param>
		private void Subscribe(MenuDisplaysPresenterDisplay display)
		{
			display.OnRefreshNeeded += DisplayOnRefreshNeeded;
		}

		/// <summary>
		/// Unsubscribe from the display events.
		/// </summary>
		/// <param name="display"></param>
		private void Unsubscribe(MenuDisplaysPresenterDisplay display)
		{
			display.OnRefreshNeeded -= DisplayOnRefreshNeeded;
		}

		/// <summary>
		/// Called when a display requests a refresh.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DisplayOnRefreshNeeded(object sender, EventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
