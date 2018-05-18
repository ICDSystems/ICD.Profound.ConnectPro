﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuDisplaysPresenter : AbstractPresenter<IMenuDisplaysView>, IMenuDisplaysPresenter
	{
		private const int MAX_LINE_WIDTH = 20;

		public event MenuDestinationPressedCallback OnDestinationPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private ISource m_ActiveSource;

		private ISource m_Display1RoutedSource;
		private bool m_Display1ActiveAudio;
		private IDestination m_Display1Destination;

		private ISource m_Display2RoutedSource;
		private bool m_Display2ActiveAudio;
		private IDestination m_Display2Destination;

		/// <summary>
		/// Gets/sets the source that is actively selected for routing.
		/// </summary>
		public ISource ActiveSource
		{
			get { return m_ActiveSource; }
			set
			{
				if (value == m_ActiveSource)
					return;

				m_ActiveSource = value;

				RefreshIfVisible();
			}
		}

		#region Display 1

		/// <summary>
		/// Gets/sets the destination for this presenter.
		/// </summary>
		public IDestination Display1Destination
		{
			get
			{
				if (m_Display1Destination == null && Room != null)
					Room.Routing.GetDisplayDestinations().TryElementAt(0, out m_Display1Destination);
				return m_Display1Destination;
			}
		}

		/// <summary>
		/// Gets/sets the source that is currently routed to the display.
		/// </summary>
		public ISource Display1RoutedSource
		{
			get { return m_Display1RoutedSource; }
			set
			{
				if (value == m_Display1RoutedSource)
					return;

				m_Display1RoutedSource = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets whether the routed source is currently audible in the room.
		/// </summary>
		public bool Display1ActiveAudio
		{
			get { return m_Display1ActiveAudio; }
			set
			{
				if (value == m_Display1ActiveAudio)
					return;

				m_Display1ActiveAudio = value;

				RefreshIfVisible();
			}
		}

		#endregion

		#region Display 2

		/// <summary>
		/// Gets/sets the destination for this presenter.
		/// </summary>
		public IDestination Display2Destination
		{
			get
			{
				if (m_Display2Destination == null && Room != null)
					Room.Routing.GetDisplayDestinations().TryElementAt(1, out m_Display2Destination);
				return m_Display2Destination;
			}
		}

		/// <summary>
		/// Gets/sets the source that is currently routed to the display.
		/// </summary>
		public ISource Display2RoutedSource
		{
			get { return m_Display2RoutedSource; }
			set
			{
				if (value == m_Display2RoutedSource)
					return;

				m_Display2RoutedSource = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Gets/sets whether the routed source is currently audible in the room.
		/// </summary>
		public bool Display2ActiveAudio
		{
			get { return m_Display2ActiveAudio; }
			set
			{
				if (value == m_Display2ActiveAudio)
					return;

				m_Display2ActiveAudio = value;

				RefreshIfVisible();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public MenuDisplaysPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDestinationPressed = null;

			base.Dispose();
		}

		protected override void Refresh(IMenuDisplaysView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				bool combine = Room != null && Room.IsCombineRoom();

				string display1DestinationName =
					Display1Destination == null
						? string.Empty
						: Display1Destination.GetName(combine) ?? string.Empty;

				bool display1HasControl = HasDeviceControl(m_Display1RoutedSource);

				eDisplayColor display1Color = m_ActiveSource == null || m_ActiveSource == m_Display1RoutedSource
					                              ? m_Display1RoutedSource == null
						                                ? eDisplayColor.Grey
						                                : display1HasControl
							                                  ? eDisplayColor.Green
							                                  : eDisplayColor.White
					                              : eDisplayColor.Yellow;

				string display1Text = m_ActiveSource == null || m_ActiveSource == m_Display1RoutedSource
					                      ? display1DestinationName
					                      : string.Format("PRESS TO SHOW SELECTION ON {0}", display1DestinationName);
				display1Text = display1Text.ToUpper();

				string display1SourceName = m_Display1RoutedSource == null
					                            ? string.Empty
					                            : m_Display1RoutedSource.GetNameOrDeviceName(combine);

				string display1Line1;
				string display1Line2;

				if (display1Text.Length <= MAX_LINE_WIDTH)
				{
					display1Line1 = display1Text;
					display1Line2 = string.Empty;
				}
				else
				{
					// Find the space closest to the middle of the text and split.
					int middleIndex = display1Text.Length / 2;
					int splitIndex = display1Text.FindIndices(char.IsWhiteSpace).GetClosest(i => i - middleIndex);

					display1Line1 = display1Text.Substring(0, splitIndex).Trim();
					display1Line2 = display1Text.Substring(splitIndex + 1).Trim();
				}

				if (display1HasControl)
				{
					display1Line1 = "PRESS FOR CONTROLS";
					display1Line2 = string.Empty;
				}

				// Icon
				ConnectProSource display1Source = m_Display1RoutedSource as ConnectProSource;
				string display1Icon = display1Source == null ? null : display1Source.Icon;
				display1Icon = Icons.GetDisplayIcon(display1Icon, display1Color);

				// Text
				string display1HexColor = Colors.DisplayColorToTextColor(display1Color);
				display1SourceName = HtmlUtils.FormatColoredText(display1SourceName, display1HexColor);
				display1Line1 = HtmlUtils.FormatColoredText(display1Line1, display1HexColor);
				display1Line2 = HtmlUtils.FormatColoredText(display1Line2, display1HexColor);

				// Speaker visibility
				bool display1ShowSpeaker =
					(m_ActiveSource == null || m_ActiveSource == m_Display1RoutedSource) &&
					m_Display1RoutedSource != null &&
					m_Display1RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio);

				view.SetDisplay1Color(display1Color);
				view.SetDisplay1SourceText(display1SourceName);
				view.SetDisplay1Line1Text(display1Line1);
				view.SetDisplay1Line2Text(display1Line2);
				view.SetDisplay1Icon(display1Icon);
				view.ShowDisplay1SpeakerButton(display1ShowSpeaker);
				view.SetDisplay1SpeakerButtonActive(m_Display1ActiveAudio);





				string display2DestinationName =
					Display2Destination == null
						? string.Empty
						: Display2Destination.GetName(combine) ?? string.Empty;

				bool display2HasControl = HasDeviceControl(m_Display2RoutedSource);

				eDisplayColor display2Color = m_ActiveSource == null || m_ActiveSource == m_Display2RoutedSource
					                              ? m_Display2RoutedSource == null
						                                ? eDisplayColor.Grey
						                                : display2HasControl
							                                  ? eDisplayColor.Green
							                                  : eDisplayColor.White
					                              : eDisplayColor.Yellow;

				string display2Text = m_ActiveSource == null || m_ActiveSource == m_Display2RoutedSource
					                      ? display2DestinationName
					                      : string.Format("PRESS TO SHOW SELECTION ON {0}", display2DestinationName);
				display2Text = display2Text.ToUpper();

				string display2SourceName = m_Display2RoutedSource == null
					                            ? string.Empty
					                            : m_Display2RoutedSource.GetNameOrDeviceName(combine);

				string display2Line1;
				string display2Line2;

				if (display2Text.Length <= MAX_LINE_WIDTH)
				{
					display2Line1 = display2Text;
					display2Line2 = string.Empty;
				}
				else
				{
					// Find the space closest to the middle of the text and split.
					int middleIndex = display2Text.Length / 2;
					int splitIndex = display2Text.FindIndices(char.IsWhiteSpace).GetClosest(i => i - middleIndex);

					display2Line1 = display2Text.Substring(0, splitIndex).Trim();
					display2Line2 = display2Text.Substring(splitIndex + 1).Trim();
				}

				if (display2HasControl)
				{
					display2Line1 = "PRESS FOR CONTROLS";
					display2Line2 = string.Empty;
				}

				// Icon
				ConnectProSource display2Source = m_Display2RoutedSource as ConnectProSource;
				string display2Icon = display2Source == null ? null : display2Source.Icon;
				display2Icon = Icons.GetDisplayIcon(display2Icon, display2Color);

				// Text
				string display2HexColor = Colors.DisplayColorToTextColor(display2Color);
				display2SourceName = HtmlUtils.FormatColoredText(display2SourceName, display2HexColor);
				display2Line1 = HtmlUtils.FormatColoredText(display2Line1, display2HexColor);
				display2Line2 = HtmlUtils.FormatColoredText(display2Line2, display2HexColor);

				// Speaker visibility
				bool display2ShowSpeaker =
					(m_ActiveSource == null || m_ActiveSource == m_Display2RoutedSource) &&
					m_Display2RoutedSource != null &&
				    m_Display2RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio);

				view.SetDisplay2Color(display2Color);
				view.SetDisplay2SourceText(display2SourceName);
				view.SetDisplay2Line1Text(display2Line1);
				view.SetDisplay2Line2Text(display2Line2);
				view.SetDisplay2Icon(display2Icon);
				view.ShowDisplay2SpeakerButton(display2ShowSpeaker);
				view.SetDisplay2SpeakerButtonActive(m_Display2ActiveAudio);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void SetRouting(Dictionary<IDestination, IcdHashSet<ISource>> routing, IcdHashSet<ISource> activeAudio)
		{
			Display1RoutedSource = GetRoutedSource(Display1Destination, routing);
			Display2RoutedSource = GetRoutedSource(Display2Destination, routing);

			Display1ActiveAudio = Display1RoutedSource != null && activeAudio.Contains(Display1RoutedSource);
			Display2ActiveAudio = Display2RoutedSource != null && activeAudio.Contains(Display2RoutedSource);
		}

		private ISource GetRoutedSource(IDestination destination, Dictionary<IDestination, IcdHashSet<ISource>> routing)
		{
			if (destination == null)
				return null;

			if (!routing.ContainsKey(destination))
				return null;

			return routing[destination].OrderBy(s => s.Order)
			                           .ThenBy(s => s.Id)
			                           .FirstOrDefault();
		}

		/// <summary>
		/// Returns the control that can be manipulated by the user, e.g. dialing control, tv tuner, etc.
		/// </summary>
		/// <returns></returns>
		private bool HasDeviceControl(ISource source)
		{
			return Room != null &&
			       source != null &&
			       Room.Routing.CanControl(source);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IMenuDisplaysView view)
		{
			base.Subscribe(view);

			view.OnDisplay1ButtonPressed += ViewOnDisplay1ButtonPressed;
			view.OnDisplay1SpeakerButtonPressed += ViewOnDisplay1SpeakerButtonPressed;
			view.OnDisplay2ButtonPressed += ViewOnDisplay2ButtonPressed;
			view.OnDisplay2SpeakerButtonPressed += ViewOnDisplay2SpeakerButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IMenuDisplaysView view)
		{
			base.Unsubscribe(view);

			view.OnDisplay1ButtonPressed -= ViewOnDisplay1ButtonPressed;
			view.OnDisplay1SpeakerButtonPressed -= ViewOnDisplay1SpeakerButtonPressed;
			view.OnDisplay2ButtonPressed -= ViewOnDisplay2ButtonPressed;
			view.OnDisplay2SpeakerButtonPressed -= ViewOnDisplay2SpeakerButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the display button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDisplay1ButtonPressed(object sender, EventArgs eventArgs)
		{
			MenuDestinationPressedCallback handler = OnDestinationPressed;
			if (handler != null)
				handler(this, Display1RoutedSource, Display1Destination);
		}

		/// <summary>
		/// Called when the user presses the speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDisplay1SpeakerButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			if (m_Display1RoutedSource == null || !m_Display1RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio))
				return;

			Room.Routing.RouteAudio(m_Display1RoutedSource);
		}

		/// <summary>
		/// Called when the user presses the display button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDisplay2ButtonPressed(object sender, EventArgs eventArgs)
		{
			MenuDestinationPressedCallback handler = OnDestinationPressed;
			if (handler != null)
				handler(this, Display2RoutedSource, Display2Destination);
		}

		/// <summary>
		/// Called when the user presses the speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnDisplay2SpeakerButtonPressed(object sender, EventArgs eventArgs)
		{
			if (Room == null)
				return;

			if (m_Display2RoutedSource == null || !m_Display2RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio))
				return;

			Room.Routing.RouteAudio(m_Display2RoutedSource);
		}

		#endregion
	}
}
