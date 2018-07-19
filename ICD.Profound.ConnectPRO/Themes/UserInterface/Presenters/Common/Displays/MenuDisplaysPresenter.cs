using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
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
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuDisplaysPresenter : AbstractPresenter<IMenuDisplaysView>, IMenuDisplaysPresenter
	{
		private const int MAX_LINE_WIDTH = 20;

		public event MenuDestinationPressedCallback OnDestinationPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private ISource m_ActiveSource;

		private readonly MenuDisplaysPresenterDisplay m_Display1;
		private readonly MenuDisplaysPresenterDisplay m_Display2;

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

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public MenuDisplaysPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_Display1 = new MenuDisplaysPresenterDisplay();
			m_Display1.OnPropertyChange += Display1OnPropertyChange;

			m_Display2 = new MenuDisplaysPresenterDisplay();
			m_Display2.OnPropertyChange += Display2OnPropertyChange;
		}

		private void Display1OnPropertyChange(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
		}

		private void Display2OnPropertyChange(object sender, EventArgs eventArgs)
		{
			RefreshIfVisible();
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
			base.SetRoom(room);

			bool combine = room != null && room.IsCombineRoom();

			m_Display1.RoomCombine = combine;
			m_Display2.RoomCombine = combine;
		}

		protected override void Refresh(IMenuDisplaysView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				/*
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

				// Text
				string display1HexColor = Colors.DisplayColorToTextColor(display1Color);
				display1SourceName = HtmlUtils.FormatColoredText(display1SourceName, display1HexColor);
				display1Line1 = HtmlUtils.FormatColoredText(display1Line1, display1HexColor);
				display1Line2 = HtmlUtils.FormatColoredText(display1Line2, display1HexColor);
				 */

				// Display 1
				view.SetDisplay1Color(m_Display1.Color);
				view.SetDisplay1SourceText(m_Display1.SourceName);
				view.SetDisplay1Line1Text(m_Display1.Line1);
				view.SetDisplay1Line2Text(m_Display1.Line2);
				view.SetDisplay1Icon(m_Display1.Icon);
				view.ShowDisplay1SpeakerButton(m_Display1.ShowSpeaker);
				view.SetDisplay1SpeakerButtonActive(m_Display1.AudioActive);

				// Display 2
				view.SetDisplay2Color(m_Display2.Color);
				view.SetDisplay2SourceText(m_Display2.SourceName);
				view.SetDisplay2Line2Text(m_Display2.Line1);
				view.SetDisplay2Line2Text(m_Display2.Line2);
				view.SetDisplay2Icon(m_Display2.Icon);
				view.ShowDisplay2SpeakerButton(m_Display2.ShowSpeaker);
				view.SetDisplay2SpeakerButtonActive(m_Display2.AudioActive);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void SetRouting(Dictionary<IDestination, IcdHashSet<ISource>> routing, IcdHashSet<ISource> activeAudio)
		{
			m_Display1.RoutedSource = GetRoutedSource(m_Display1.Destination, routing);
			m_Display1.AudioActive = m_Display1.RoutedSource != null && activeAudio.Contains(m_Display1.RoutedSource);

			m_Display2.RoutedSource = GetRoutedSource(m_Display2.Destination, routing);
			m_Display2.AudioActive = m_Display2.RoutedSource != null && activeAudio.Contains(m_Display2.RoutedSource);
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
				handler(this, m_Display1.RoutedSource, m_Display1.Destination);
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

			if (m_Display1.RoutedSource == null || !m_Display1.RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio))
				return;

			Room.Routing.RouteAudio(m_Display1.RoutedSource);
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
				handler(this, m_Display2.RoutedSource, m_Display2.Destination);
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

			if (m_Display2.RoutedSource == null || !m_Display2.RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio))
				return;

			Room.Routing.RouteAudio(m_Display2.RoutedSource);
		}

		#endregion
	}
}
