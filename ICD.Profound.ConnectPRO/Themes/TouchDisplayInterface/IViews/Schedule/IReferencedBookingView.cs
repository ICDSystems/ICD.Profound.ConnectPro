namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule
{
	public interface IReferencedBookingView : ITouchDisplayView
	{
		/// <summary>
		/// Sets the text for the time span of the schedule item.
		/// </summary>
		/// <param name="text"></param>
		void SetTimeLabel(string text);

		/// <summary>
		/// Sets the text for the subject of the schedule item.
		/// </summary>
		/// <param name="text"></param>
		void SetSubjectLabel(string text);
	}
}