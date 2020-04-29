namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Welcome
{
	public interface IReferencedScheduleView : IOsdView
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