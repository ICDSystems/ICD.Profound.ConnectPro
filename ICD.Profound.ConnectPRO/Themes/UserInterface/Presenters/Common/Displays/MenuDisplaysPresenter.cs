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
	public sealed class MenuDisplaysPresenter : AbstractUiPresenter<IMenuDisplaysView>, IMenuDisplaysPresenter
	{
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

				bool refresh = false;

				refresh |= m_Display1.SetActiveSource(m_ActiveSource);
				refresh |= m_Display2.SetActiveSource(m_ActiveSource);

				if (refresh)
					RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public MenuDisplaysPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_Display1 = new MenuDisplaysPresenterDisplay();
			m_Display2 = new MenuDisplaysPresenterDisplay();
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

			m_Display1.SetRoomCombine(combine);
			m_Display2.SetRoomCombine(combine);

			IDestination[] destinations = room == null ? new IDestination[0] : room.Routing.GetDisplayDestinations().ToArray();

			IDestination dest1;
			destinations.TryElementAt(0, out dest1);
			m_Display1.SetDestination(dest1);

			IDestination dest2;
			destinations.TryElementAt(1, out dest2);
			m_Display2.SetDestination(dest2);

			base.SetRoom(room);
		}

		protected override void Refresh(IMenuDisplaysView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
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
				view.SetDisplay2Line1Text(m_Display2.Line1);
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
			bool refresh = false;

			refresh |= m_Display1.SetRoutedSource(GetRoutedSource(m_Display1.Destination, routing));
			refresh |= m_Display1.SetAudioActive(m_Display1.RoutedSource != null && activeAudio.Contains(m_Display1.RoutedSource));

			refresh |= m_Display2.SetRoutedSource(GetRoutedSource(m_Display2.Destination, routing));
			refresh |= m_Display2.SetAudioActive(m_Display2.RoutedSource != null && activeAudio.Contains(m_Display2.RoutedSource));

			if (refresh)
				RefreshIfVisible();
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
