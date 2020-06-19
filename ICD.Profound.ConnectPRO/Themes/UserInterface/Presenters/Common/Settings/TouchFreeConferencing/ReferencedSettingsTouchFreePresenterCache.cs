using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Settings.Originators;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.TouchFreeConferencing
{
	public sealed class ReferencedSettingsTouchFreePresenterCache
	{
		private IRoom m_Room;
		private string m_Text;
		private bool m_Combined;
		private string m_FeedbackPlain;

		#region Properties

		public ISource Source { get; private set; }
		public bool Selected { get; private set; }
		public string Icon { get; private set; }
		public string Text { get; private set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ReferencedSettingsTouchFreePresenterCache()
		{
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

			m_Text = text;

			UpdateFeedback();
			UpdateText();

			return true;
		}

		public bool SetSelected(bool selected)
		{
			if (selected == Selected)
				return false;

			Selected = selected;

			return true;

		}

		private void UpdateText()
		{
			throw new System.NotImplementedException();
		}

		private void UpdateFeedback()
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}