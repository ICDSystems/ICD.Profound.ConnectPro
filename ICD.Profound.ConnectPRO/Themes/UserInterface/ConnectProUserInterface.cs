using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.Cisco.Controls;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Routing.Extensions;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VisibilityTree;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class ConnectProUserInterface : IUserInterface
	{
		private readonly IPanelDevice m_Panel;

		private readonly INavigationController m_NavigationController;

		private IConnectProRoom m_Room;

		private DefaultVisibilityNode m_RootVisibility;
		private ISource m_ActiveSource;
		private IRoutingGraph m_RoutingGraph;

		#region Properties

		public IPanelDevice Panel { get { return m_Panel; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProUserInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			m_Panel = panel;
			UpdatePanelOnlineJoin();

			IViewFactory viewFactory = new ConnectProViewFactory(panel, theme);
			m_NavigationController = new ConnectProNavigationController(viewFactory, theme);

			BuildVisibilityTree();
			SubscribePresenters();
		}

		/// <summary>
		/// Updates the "offline" visual state of the panel
		/// </summary>
		private void UpdatePanelOnlineJoin()
		{
			m_Panel.SendInputDigital(CommonJoins.DIGITAL_OFFLINE_JOIN, m_Room == null);
		}

		/// <summary>
		/// Builds the rules for view visibility, e.g. prevent certain items from being visible at the same time.
		/// </summary>
		private void BuildVisibilityTree()
		{
			// Only allow one of the start/end buttons to be visible at any given time
			m_RootVisibility = new DefaultVisibilityNode(m_NavigationController.LazyLoadPresenter<IStartMeetingPresenter>());

			// Video Conference node
			IVisibilityNode videoConferencingVisibility =
				new SingleVisibilityNode();

			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcContactsPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcCameraPresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcSharePresenter>());
			videoConferencingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcDtmfPresenter>());

			IVisibilityNode meetingVisibility = new VisibilityNode();
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IEndMeetingPresenter>());
			meetingVisibility.AddNode(videoConferencingVisibility);
			meetingVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IVtcBasePresenter>());

			m_RootVisibility.AddNode(meetingVisibility);

			// These presenters are initially visible.
			m_NavigationController.NavigateTo<IHeaderPresenter>();
			m_NavigationController.NavigateTo<IHardButtonsPresenter>();

			// These presenters control their own visibility.
			m_NavigationController.LazyLoadPresenter<IEndMeetingPresenter>();
			m_NavigationController.LazyLoadPresenter<IStartMeetingPresenter>();
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			UnsubscribePresenters();

			SetRoom(null);

			m_NavigationController.Dispose();
		}

		/// <summary>
		/// Updates the UI to represent the given room.
		/// </summary>
		/// <param name="room"></param>
		public void SetRoom(IConnectProRoom room)
		{
			if (room == m_Room)
				return;

			Unsubscribe(m_RoutingGraph);
			Unsubscribe(m_Room);

			ServiceProvider.GetService<ILoggerService>()
			               .AddEntry(eSeverity.Informational, "{0} setting room to {1}", this, room);

			m_Room = room;
			m_RoutingGraph = m_Room == null ? null : m_Room.Core.GetRoutingGraph();

			m_NavigationController.SetRoom(room);

			Subscribe(m_RoutingGraph);
			Subscribe(m_Room);

			UpdatePanelOnlineJoin();
			UpdateMeetingPresenters();
		}

		/// <summary>
		/// Gets the string representation for this instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			ReprBuilder builder = new ReprBuilder(this);
			builder.AppendProperty("Panel", m_Panel);
			return builder.ToString();
		}

		#endregion

		#region Room Callbacks

		/// <summary>
		/// Subscribe to the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Subscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged += RoomOnIsInMeetingChanged;
		}

		/// <summary>
		/// Unsubscribe from the room events.
		/// </summary>
		/// <param name="room"></param>
		private void Unsubscribe(IConnectProRoom room)
		{
			if (room == null)
				return;

			room.OnIsInMeetingChanged -= RoomOnIsInMeetingChanged;
		}

		/// <summary>
		/// Called when the room enters/exits a meeting.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateMeetingPresenters();
		}

		/// <summary>
		/// Sets the visibility of the subpages based on the meeting state.
		/// </summary>
		private void UpdateMeetingPresenters()
		{
			bool isInMeeting = m_Room != null && m_Room.IsInMeeting;

			// Set the visibility of the meeting buttons
			m_NavigationController.LazyLoadPresenter<IStartMeetingPresenter>().ShowView(!isInMeeting);
			m_NavigationController.LazyLoadPresenter<IEndMeetingPresenter>().ShowView(isInMeeting);

			IDestination[] displayDestinations =
				m_Room == null ? new IDestination[0] : m_Room.Routing.GetDisplayDestinations().ToArray();

			// Set the visibility of the source a display subpages
			bool dualSourceVisible = isInMeeting && displayDestinations.Length > 1;
			bool singleSourceVisible = isInMeeting && !dualSourceVisible;
			bool displaysVisible = isInMeeting && dualSourceVisible;

			m_NavigationController.LazyLoadPresenter<ISourceSelectSinglePresenter>().ShowView(singleSourceVisible);
			m_NavigationController.LazyLoadPresenter<ISourceSelectDualPresenter>().ShowView(dualSourceVisible);
			m_NavigationController.LazyLoadPresenter<IDisplaysPresenter>().ShowView(displaysVisible);
		}

		#endregion

		#region RoutingGraph Callbacks

		/// <summary>
		/// Subscribe to the routing graph events.
		/// </summary>
		/// <param name="routingGraph"></param>
		private void Subscribe(IRoutingGraph routingGraph)
		{
			if (routingGraph == null)
				return;

			routingGraph.OnRouteChanged += RoutingGraphOnRouteChanged;
			routingGraph.OnSourceDetectionStateChanged += RoutingGraphOnSourceDetectionStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the routing graph events.
		/// </summary>
		/// <param name="routingGraph"></param>
		private void Unsubscribe(IRoutingGraph routingGraph)
		{
			if (routingGraph == null)
				return;

			routingGraph.OnRouteChanged -= RoutingGraphOnRouteChanged;
			routingGraph.OnSourceDetectionStateChanged -= RoutingGraphOnSourceDetectionStateChanged;
		}

		/// <summary>
		/// Called when a switcher changes routing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RoutingGraphOnRouteChanged(object sender, EventArgs args)
		{
			if (m_Room == null)
				return;

			Dictionary<IDestination, ISource> routing = new Dictionary<IDestination, ISource>();

			foreach (ISource source in m_Room.Routing.GetCoreSources())
			{
				foreach (IDestination destination in m_Room.Routing.GetActiveDisplayDestinations(source))
					routing[destination] = source;
			}

			m_NavigationController.LazyLoadPresenter<ISourceSelectSinglePresenter>().SetRoutedSources(routing);
			m_NavigationController.LazyLoadPresenter<ISourceSelectDualPresenter>().SetRoutedSources(routing);
			m_NavigationController.LazyLoadPresenter<IDisplaysPresenter>().SetRoutedSources(routing);
		}

		/// <summary>
		/// Called when a source is detected or loses detection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void RoutingGraphOnSourceDetectionStateChanged(object sender, EndpointStateEventArgs args)
		{
			// TODO - Update source presenter states
		}

		#endregion

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		private void SubscribePresenters()
		{
			Subscribe(m_NavigationController.LazyLoadPresenter<ISourceSelectSinglePresenter>());
			Subscribe(m_NavigationController.LazyLoadPresenter<ISourceSelectDualPresenter>());
			Subscribe(m_NavigationController.LazyLoadPresenter<IDisplaysPresenter>());
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		private void UnsubscribePresenters()
		{
			Unsubscribe(m_NavigationController.LazyLoadPresenter<ISourceSelectSinglePresenter>());
			Unsubscribe(m_NavigationController.LazyLoadPresenter<ISourceSelectDualPresenter>());
			Unsubscribe(m_NavigationController.LazyLoadPresenter<IDisplaysPresenter>());
		}

		#region SourceSelectSingle Callbacks

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(ISourceSelectSinglePresenter presenter)
		{
			presenter.OnSourcePressed += SourceSelectSinglePresenterOnSourcePressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(ISourceSelectSinglePresenter presenter)
		{
			presenter.OnSourcePressed -= SourceSelectSinglePresenterOnSourcePressed;
		}

		/// <summary>
		/// Called when the user presses a source in the presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="source"></param>
		private void SourceSelectSinglePresenterOnSourcePressed(object sender, ISource source)
		{
			if (m_Room == null)
				return;

			m_Room.Routing.Route(source);

			SetActiveSource(null);
		}

		#endregion

		#region SourceSelectDual Callbacks

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(ISourceSelectDualPresenter presenter)
		{
			presenter.OnSourcePressed += SourceSelectDualPresenterOnSourcePressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(ISourceSelectDualPresenter presenter)
		{
			presenter.OnSourcePressed -= SourceSelectDualPresenterOnSourcePressed;
		}

		/// <summary>
		/// Called when the user presses a source in the presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="source"></param>
		private void SourceSelectDualPresenterOnSourcePressed(object sender, ISource source)
		{
			// Toggle if the given source is already active
			if (source == m_ActiveSource)
				source = null;

			SetActiveSource(source);
		}

		/// <summary>
		/// Sets the source that is currently active for routing to the displays.
		/// </summary>
		/// <param name="source"></param>
		private void SetActiveSource(ISource source)
		{
			if (source == m_ActiveSource)
				return;

			m_ActiveSource = source;

			m_NavigationController.LazyLoadPresenter<ISourceSelectDualPresenter>().ActiveSource = m_ActiveSource;
			m_NavigationController.LazyLoadPresenter<IDisplaysPresenter>().ActiveSource = m_ActiveSource;
		}

		#endregion

		#region Displays Callbacks

		/// <summary>
		/// Subscribe to the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Subscribe(IDisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed += DisplaysPresenterOnDestinationPressed;
		}

		/// <summary>
		/// Unsubscribe from the presenter events.
		/// </summary>
		/// <param name="presenter"></param>
		private void Unsubscribe(IDisplaysPresenter presenter)
		{
			presenter.OnDestinationPressed -= DisplaysPresenterOnDestinationPressed;
		}

		/// <summary>
		/// Called when the user presses a destination in the presenter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="presenter"></param>
		/// <param name="destination"></param>
		private void DisplaysPresenterOnDestinationPressed(object sender, IReferencedDisplaysPresenter presenter, IDestination destination)
		{
			if (m_Room == null)
				return;

			if (m_ActiveSource != null)
			{
				m_Room.Routing.Route(m_ActiveSource, destination);
				SetActiveSource(null);
				return;
			}

			// Contextual
			ISource routedSource = presenter.RoutedSource;
			IRouteSourceControl sourceControl = m_Room.Core.GetControl<IRouteSourceControl>(routedSource.Endpoint.Device, routedSource.Endpoint.Control);

			// TODO - VERY temporary
			CiscoCodecRoutingControl codecControl = sourceControl as CiscoCodecRoutingControl;
			if (codecControl != null)
				m_NavigationController.NavigateTo<IVtcBasePresenter>();
		}

		#endregion
	}
}
