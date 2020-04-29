using System;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference
{
	public interface IRecordConferenceView : ITouchDisplayView
	{
		/// <summary>
		/// Raised when the record button is pressed.
		/// </summary>
		event EventHandler OnRecordButtonPressed;

		/// <summary>
		/// Raised when the stop button is pressed.
		/// </summary>
		event EventHandler OnStopButtonPressed;

		/// <summary>
		/// Sets the enabled state of the record button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetRecordButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the selected state of the record button.
		/// </summary>
		/// <param name="selected"></param>
		void SetRecordButtonSelected(bool selected);

		/// <summary>
		/// Sets the enabled state of the stop button.
		/// </summary>
		/// <param name="enabled"></param>
		void SetStopButtonEnabled(bool enabled);

		/// <summary>
		/// Sets the animation state of the recording graphic.
		/// </summary>
		/// <param name="animate"></param>
		void SetRecordAnimation(bool animate);
	}
}