using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuDisplaysPresenterDisplay
	{
		public event EventHandler OnPropertyChange;

		private eDisplayColor m_Color;
		private string m_SourceName;
		private string m_Line1;
		private string m_Line2;
		private string m_Icon;
		private bool m_ShowSpeaker;
		private bool m_AudioActive;
		private ISource m_RoutedSource;

		public eDisplayColor Color { get { return m_Color; } }
		public string SourceName { get { return m_SourceName; } }
		public string Line1 { get { return m_Line1; } }
		public string Line2 { get { return m_Line2; } }

		public string Icon { get; private set; }

		public bool ShowSpeaker
		{
			get
			{
				return (m_ActiveSource == null || m_ActiveSource == m_RoutedSource) &&
					   m_RoutedSource != null &&
					   m_RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio);
			}
		}

		public bool AudioActive
		{
			get
			{
				return m_AudioActive;
			}
			set
			{
				if (value == m_AudioActive)
					return;

				m_AudioActive = value;

				OnPropertyChange.Raise(this);
			}
		}

		public bool HasControl { get { return HasDeviceControl(m_RoutedSource); } }

		public IDestination Destination { get; set; }

		public ISource RoutedSource
		{
			get { return m_RoutedSource; }
			set
			{
				if (value == m_RoutedSource)
					return;

				m_RoutedSource = value;

				// Update icon
				ConnectProSource source = RoutedSource as ConnectProSource;
				string display1Icon = source == null ? null : source.Icon;
				Icon = Icons.GetDisplayIcon(display1Icon, Color);

				// Update control state
				HasControl = m_RoutedSource != null && ConnectProRouting.CanControl(m_RoutedSource);

				// Show speaker
				return (m_ActiveSource == null || m_ActiveSource == m_RoutedSource) &&
					   m_RoutedSource != null &&
					   m_RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio);

				OnPropertyChange.Raise(this);
			}
		}

		public string Name { get; }

		public bool RoomCombine
		{
			get; set;
		}
	}
}
