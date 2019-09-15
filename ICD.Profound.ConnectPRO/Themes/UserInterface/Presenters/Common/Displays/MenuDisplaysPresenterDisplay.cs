using ICD.Common.Utils.Extensions;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Endpoints.Destinations;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Routing.Endpoints.Sources;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuDisplaysPresenterDisplay
	{
		private const int MAX_LINE_WIDTH = 20;

		private eDisplayColor m_Color;
		private string m_SourceName;
		private string m_Line1;
		private string m_Line2;
		private bool m_AudioActive;
		private ISource m_RoutedSource;
		private ISource m_SelectedSource;
		private IDestinationBase m_Destination;
		private bool m_RoomCombine;
		private bool m_HasControl;
		private bool m_ShowSpeaker;
		private bool m_CanRouteToRoomAudio;
		private string m_Icon;
		private bool m_CanRouteVideo;

		#region Properties

		public eDisplayColor Color { get { return m_Color; } }
		public string SourceName { get { return m_SourceName; } }
		public string Line1 { get { return m_Line1; } }
		public string Line2 { get { return m_Line2; } }

		public string Icon { get { return m_Icon; } }

		public bool ShowSpeaker { get { return m_ShowSpeaker; } }

		public bool AudioActive { get { return m_AudioActive; } }

		public IDestinationBase Destination { get { return m_Destination; } }

		public ISource RoutedSource { get { return m_RoutedSource; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public MenuDisplaysPresenterDisplay()
		{
			m_Color = eDisplayColor.Grey;
		}

		#region Methods

		public bool SetRoutedSource(ISource routedSource, bool canRouteToRoomAudio)
		{
			if (routedSource == m_RoutedSource &&
			    canRouteToRoomAudio == m_CanRouteToRoomAudio)
				return false;

			m_RoutedSource = routedSource;
			m_CanRouteToRoomAudio = canRouteToRoomAudio;

			// Update has control
			UpdateHasControl();

			// Update the color
			UpdateColor();

			// Update icon
			UpdateIcon();

			// Update the labels
			UpdateLabels();

			// Update show speaker
			UpdateShowSpeaker();

			return true;
		}

		public bool SetSelectedSource(ISource selectedSource, bool canRouteVideo)
		{
			if (selectedSource == m_SelectedSource && canRouteVideo == m_CanRouteVideo)
				return false;

			m_SelectedSource = selectedSource;
			m_CanRouteVideo = canRouteVideo;

			// Update the color
			UpdateColor();

			// Update icon
			UpdateIcon();

			// Update the labels
			UpdateLabels();

			// Update show speaker
			UpdateShowSpeaker();

			return true;
		}

		public bool SetAudioActive(bool value)
		{
			if (value == m_AudioActive)
				return false;

			m_AudioActive = value;

			return true;
		}

		public bool SetDestination(IDestinationBase destination)
		{
			if (destination == m_Destination)
				return false;

			m_Destination = destination;

			// Update the labels
			UpdateLabels();

			return true;
		}

		public bool SetRoomCombine(bool combine)
		{
			if (combine == m_RoomCombine)
				return false;

			m_RoomCombine = true;

			// Update the labels
			UpdateLabels();

			return true;
		}

		#endregion

		#region Private Methods

		private void UpdateHasControl()
		{
			m_HasControl = m_RoutedSource != null && ConnectProRoutingSources.CanControl(m_RoutedSource);
		}

		private void UpdateColor()
		{
			m_Color = m_SelectedSource == null || m_SelectedSource == m_RoutedSource
				          ? m_RoutedSource == null
					            ? eDisplayColor.Grey
					            : m_HasControl
						              ? eDisplayColor.Green
						              : eDisplayColor.White
				          : m_CanRouteVideo
					            ? eDisplayColor.Yellow
					            : eDisplayColor.Grey;
		}

		private void UpdateIcon()
		{

			ConnectProSource source = m_RoutedSource as ConnectProSource;
			string icon = source == null ? null : source.Icon;
			m_Icon = Icons.GetDisplayIcon(icon, m_Color);
		}

		private void UpdateLabels()
		{
			if (m_HasControl)
			{
				m_Line1 = "PRESS FOR CONTROLS";
				m_Line2 = string.Empty;
			}
			else
			{
				string destinationName =
					m_Destination == null
						? string.Empty
						: m_Destination.GetName(m_RoomCombine) ?? string.Empty;

				string text = m_SelectedSource == null || m_SelectedSource == m_RoutedSource
					              ? destinationName
					              : m_CanRouteVideo
						              ? string.Format("PRESS TO SHOW SELECTION ON {0}", destinationName)
						              : string.Format("UNABLE TO SHOW SELECTION ON {0}", destinationName);
				text = text.ToUpper();

				if (text.Length <= MAX_LINE_WIDTH)
				{
					m_Line1 = text;
					m_Line2 = string.Empty;
				}
				else
				{
					// Find the space closest to the middle of the text and split.
					int middleIndex = text.Length / 2;
					int splitIndex = text.FindIndices(char.IsWhiteSpace).GetClosest(i => i - middleIndex);

					m_Line1 = text.Substring(0, splitIndex).Trim();
					m_Line2 = text.Substring(splitIndex + 1).Trim();
				}
			}

			m_SourceName = m_RoutedSource == null ? string.Empty : m_RoutedSource.GetName(m_RoomCombine);

			string hexColor = Colors.DisplayColorToTextColor(m_Color);
			m_SourceName = HtmlUtils.FormatColoredText(m_SourceName, hexColor);
			m_Line1 = HtmlUtils.FormatColoredText(m_Line1, hexColor);
			m_Line2 = HtmlUtils.FormatColoredText(m_Line2, hexColor);
		}

		private void UpdateShowSpeaker()
		{
			m_ShowSpeaker = m_CanRouteToRoomAudio &&
			                (m_SelectedSource == null || m_SelectedSource == m_RoutedSource) &&
			                m_RoutedSource != null &&
			                m_RoutedSource.ConnectionType.HasFlag(eConnectionType.Audio);
		}

		#endregion
	}
}
