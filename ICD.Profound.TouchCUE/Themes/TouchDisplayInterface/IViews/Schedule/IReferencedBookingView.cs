using System;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Schedule
{
	public interface IReferencedBookingView : ITouchDisplayView
	{
		event EventHandler OnPressed;

		/// <summary>
		///     Sets the text for the time span of the schedule item.
		/// </summary>
		/// <param name="text"></param>
		void SetTimeLabel(string text);

		/// <summary>
		///     Sets the text for the subject of the schedule item.
		/// </summary>
		/// <param name="text"></param>
		void SetSubjectLabel(string text);

		void SetButtonEnabled(bool enabled);

		void SetButtonSelected(bool selected);
	}
}