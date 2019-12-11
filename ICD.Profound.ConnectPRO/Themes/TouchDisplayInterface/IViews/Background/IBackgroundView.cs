namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Background
{
	public interface IBackgroundView : ITouchDisplayView
	{
		void SetBackgroundMode(eTouchCueBackgroundMode mode);

		void SetBackgroundMotion(bool motion);
	}

	public enum eTouchCueBackgroundMode
	{
		/// <summary>
		/// Default mode, neutral/business-friendly CUE background, never changes
		/// </summary>
		Neutral = 0,
		/// <summary>
		/// New background each month, themed for month (US themes)
		/// </summary>
		Monthly = 1,
		HdmiInput = 2
	}
}