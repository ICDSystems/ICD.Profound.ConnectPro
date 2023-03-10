using System.Linq;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.ConnectPROCommon.Routing.Endpoints.Sources;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Sources
{
	public sealed class ReferencedSourceSelectPresenterCache
	{
		private const int MAX_LINE_WIDTH = 10;

		private IRoom m_Room;
		private string m_FeedbackPlain;
		private string m_HexColor;
		private string m_Line1Plain;
		private string m_Line2Plain;
		private string m_IconPlain;
		private bool m_Combined;

		#region Properties

		public ISource Source { get; private set; }
		public bool Selected { get; private set; }
		public eSourceState SourceState { get; private set; }
		public eSourceColor Color { get; private set; }
		public string Feedback { get; private set; }
		public string Line1 { get; private set; }
		public string Line2 { get; private set; }
		public string Icon { get; private set; }
		public bool SourceOnline { get; private set; }
		public bool EnableWhenOffline { get; private set; }

		public bool Enabled
		{
			get { return SourceOnline || EnableWhenOffline; }
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ReferencedSourceSelectPresenterCache()
		{
			SourceState = eSourceState.Inactive;
			
			UpdateColor();
		}

		#region Methods

		public bool SetSource(IRoom room, ISource source, bool combined)
		{
			if (source == Source &&
			    room == m_Room &&
			    combined == m_Combined)
				return false;

			Source = source;
			m_Room = room;
			m_Combined = combined;

			string sourceName =
				Source == null
					? string.Empty
					: Source.GetName(m_Combined) ?? string.Empty;
			m_FeedbackPlain = m_Room == null ? string.Empty : m_Room.GetName(m_Combined);

			string text = sourceName.ToUpper();

			m_Line1Plain = text;
			m_Line2Plain = string.Empty;

			// Find the space closest to the middle of the text and split.
			if (text.Length > MAX_LINE_WIDTH && text.Any(char.IsWhiteSpace))
			{
				int middleIndex = text.Length / 2;
				int splitIndex = text.FindIndices(char.IsWhiteSpace).GetClosest(i => i - middleIndex);

				m_Line1Plain = text.Substring(0, splitIndex).Trim();
				m_Line2Plain = text.Substring(splitIndex + 1).Trim();
			}

			ConnectProSource connectProSource = Source as ConnectProSource;
			m_IconPlain = connectProSource == null ? null : connectProSource.Icon;

			UpdateIcon();
			UpdateFeedback();
			UpdateText();

			return true;
		}

		public bool SetSelected(bool selected)
		{
			if (selected == Selected)
				return false;

			Selected = selected;

			UpdateColor();

			return true;
		}

		public bool SetRouted(eSourceState sourceState)
		{
			if (sourceState == SourceState)
				return false;

			SourceState = sourceState;

			return true;
		}

		public bool SetSourceOnline(bool sourceOnline)
		{
			if (sourceOnline == SourceOnline)
				return false;

			SourceOnline = sourceOnline;

			UpdateColor();

			return true;
		}

		public bool SetEnableWhenOffline(bool enableWhenOffline)
		{
			if (enableWhenOffline == EnableWhenOffline)
				return false;

			EnableWhenOffline = enableWhenOffline;

			UpdateColor();

			return true;
		}

		#endregion

		#region Private Methods

		private void UpdateColor()
		{

			Color = GetColor();
			m_HexColor = Colors.SourceColorToTextColor(Color);

			UpdateText();
			UpdateFeedback();
			UpdateIcon();
		}

		private eSourceColor GetColor()
		{
			if (!Enabled)
				return eSourceColor.Grey;

			return Selected ? eSourceColor.Yellow : eSourceColor.White;
		}

		private void UpdateText()
		{
			Line1 = HtmlUtils.FormatColoredText(m_Line1Plain, m_HexColor);
			Line2 = HtmlUtils.FormatColoredText(m_Line2Plain, m_HexColor);
		}

		private void UpdateFeedback()
		{
			Feedback = HtmlUtils.FormatColoredText(m_FeedbackPlain, m_HexColor);
		}

		private void UpdateIcon()
		{
			Icon = Icons.GetSourceIcon(m_IconPlain, Color);
		}

		#endregion
	}
}
