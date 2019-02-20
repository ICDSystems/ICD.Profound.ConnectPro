using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	[ViewBinding(typeof(IWtcRecordingView))]
	public sealed partial class WtcRecordingView : AbstractUiView, IWtcRecordingView
	{
		public event EventHandler OnStartRecordingButtonPressed;
		public event EventHandler OnStopRecordingButtonPressed;

		public WtcRecordingView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public override void Dispose()
		{
			OnStartRecordingButtonPressed = null;
			OnStopRecordingButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		public void SetStartRecordingButtonEnabled(bool enabled)
		{
			m_StartRecordingButton.Enable(enabled);
		}

		public void SetStopRecordingButtonEnabled(bool enabled)
		{
			m_StopRecordingButton.Enable(enabled);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_StartRecordingButton.OnPressed += StartRecordingButtonOnOnPressed;
			m_StopRecordingButton.OnPressed += StopRecordingButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_StartRecordingButton.OnPressed -= StartRecordingButtonOnOnPressed;
			m_StopRecordingButton.OnPressed -= StopRecordingButtonOnOnPressed;
		}

		private void StartRecordingButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnStartRecordingButtonPressed.Raise(this);
		}

		private void StopRecordingButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnStopRecordingButtonPressed.Raise(this);
		}

		#endregion
	}
}