﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Zoom;
using ICD.Connect.Devices;
using ICD.Connect.Panels;
using ICD.Connect.Panels.Devices;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.VisibilityTree;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Popups;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Sources;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.VisibilityTree;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IPresenters.Welcome;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Presenters;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.Views;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface
{
	/// <summary>
	/// Holds the presenter/view hierarchy for a complete panel UI.
	/// </summary>
	public sealed class ConnectProOsdInterface : IUserInterface
	{
		private readonly IPanelDevice m_Panel;
		private readonly IOsdNavigationController m_NavigationController;

		private IVisibilityNode m_DefaultNotification;
		private IVisibilityNode m_MainPageVisibility;
		private IVisibilityNode m_NotificationVisibility;

		private IConnectProRoom m_Room;

		#region Properties

		public IPanelDevice Panel { get { return m_Panel; } }

		public IConnectProRoom Room { get { return m_Room; } }

		object IUserInterface.Target { get { return Panel; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public ConnectProOsdInterface(IPanelDevice panel, ConnectProTheme theme)
		{
			m_Panel = panel;
			UpdatePanelOnlineJoin();

			IOsdViewFactory viewFactory = new ConnectProOsdViewFactory(panel, theme);
			m_NavigationController = new ConnectProOsdNavigationController(viewFactory, theme);

			BuildVisibilityTree();
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
			// Show "hello" when no notifications are visible
			m_DefaultNotification = new NotificationVisibilityNode(m_NavigationController.LazyLoadPresenter<IOsdHelloPresenter>());
			m_DefaultNotification.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdIncomingCallPresenter>());
			m_DefaultNotification.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdMutePresenter>());
			
			// show "welcome" when no other main page is visible
			m_MainPageVisibility = new SingleVisibilityNode();
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdWelcomePresenter>());
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdSourcesPresenter>());
			m_MainPageVisibility.AddPresenter(m_NavigationController.LazyLoadPresenter<IOsdConferencePresenter>());

			// these presenters are initially visible
			m_NavigationController.NavigateTo<IOsdHelloPresenter>();
			m_NavigationController.NavigateTo<IOsdHeaderPresenter>();
			
			UpdateVisibility();
		}

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
			room.Routing.State.OnSourceRoutedChanged += RoutingStateOnSourceRoutedChanged;
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
			room.Routing.State.OnSourceRoutedChanged -= RoutingStateOnSourceRoutedChanged;
		}

		private void RoomOnIsInMeetingChanged(object sender, BoolEventArgs eventArgs)
		{
			UpdateVisibility();
		}
		
		private void RoutingStateOnSourceRoutedChanged(object sender, EventArgs e)
		{
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			if (Room == null)
				return;

			IEnumerable<ISource> activeSources = Room.Routing.State.GetSourceRoutedStates()
			                                         .Where(kvp => kvp.Value == eSourceState.Active)
			                                         .Select(kvp => kvp.Key).ToList();
			bool zoomRouted = false;
			foreach (var source in activeSources)
			{
				IOriginator child;
				if (Room.Core.Originators.TryGetChild(source.Device, out child) && child is ZoomRoom)
				{
					zoomRouted = true;
					var control = (child as IDevice).Controls.GetControl<IConferenceDeviceControl>();
					m_NavigationController.LazyLoadPresenter<IOsdConferencePresenter>().ActiveConferenceControl = control;
					break;
				}
			}

			if (zoomRouted)
				m_NavigationController.NavigateTo<IOsdConferencePresenter>();
			else if (Room.IsInMeeting)
				m_NavigationController.NavigateTo<IOsdSourcesPresenter>();
			else if (Room.CalendarControl != null)
				m_NavigationController.NavigateTo<IOsdWelcomePresenter>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
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

			ServiceProvider.GetService<ILoggerService>()
			               .AddEntry(eSeverity.Informational, "{0} setting room to {1}", this, room);

			Unsubscribe(m_Room);
			m_Room = room;
			Subscribe(m_Room);

			m_NavigationController.SetRoom(room);

			UpdatePanelOnlineJoin();
			UpdateVisibility();
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
	}
}
