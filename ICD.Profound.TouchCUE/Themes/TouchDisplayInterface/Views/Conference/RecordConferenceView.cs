using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views.Conference
{
	[ViewBinding(typeof(IRecordConferenceView))]
	public sealed partial class RecordConferenceView : AbstractTouchDisplayView, IRecordConferenceView
	{
		/// <summary>
		/// Raised when the record button is pressed.
		/// </summary>
		public event EventHandler OnRecordButtonPressed;

		/// <summary>
		/// Raised when the stop button is pressed.
		/// </summary>
		public event EventHandler OnStopButtonPressed;

		public RecordConferenceView(ISigInputOutput panel, TouchCueTheme theme) : base(panel, theme)
		{
		}

		/// <summary>
		/// Sets the enabled state of the record button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetRecordButtonEnabled(bool enabled)
		{
			m_RecordButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the selected state of the record button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetRecordButtonSelected(bool selected)
		{
			m_RecordButton.SetSelected(selected);
		}

		/// <summary>
		/// Sets the enabled state of the stop button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetStopButtonEnabled(bool enabled)
		{
			m_StopButton.Enable(enabled);
		}

		/// <summary>
		/// Sets the animation state of the recording graphic.
		/// </summary>
		/// <param name="animate"></param>
		public void SetRecordAnimation(bool animate)
		{
			m_RecordingAnimation.Enable(animate);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_RecordButton.OnPressed += RecordButtonOnPressed;
			m_StopButton.OnPressed += StopButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_RecordButton.OnPressed -= RecordButtonOnPressed;
			m_StopButton.OnPressed -= StopButtonOnPressed;
		}

		private void RecordButtonOnPressed(object sender, EventArgs e)
		{
			OnRecordButtonPressed.Raise(this);
		}

		private void StopButtonOnPressed(object sender, EventArgs e)
		{
			OnStopButtonPressed.Raise(this);
		}

		#endregion
	}
}
