using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class StartMeetingView : AbstractView, IStartMeetingView
	{
		/// <summary>
		/// Raised when the user presses the start meeting button.
		/// </summary>
		public event EventHandler OnStartMeetingButtonPressed;

		/// <summary>
		/// Raised when the user presses the shutdown button.
		/// </summary>
		public event EventHandler OnShutdownButtonPressed;

		/// <summary>
		/// Sets the enabled state of the start meeting button.
		/// </summary>
		/// <param name="enabled"></param>
		public void SetStartMeetingButtonEnabled(bool enabled)
		{
			m_StartMeetingButton.SetSelected(enabled);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnStartMeetingButtonPressed = null;
			OnShutdownButtonPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public StartMeetingView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_StartMeetingButton.OnPressed += StartMeetingButtonOnPressed;
			m_ShutdownButton.OnPressed += ShutdownButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_StartMeetingButton.OnPressed -= StartMeetingButtonOnPressed;
			m_ShutdownButton.OnPressed -= ShutdownButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the shutdown button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ShutdownButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnShutdownButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the start meeting button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void StartMeetingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnStartMeetingButtonPressed.Raise(this);
		}

		#endregion
	}
}
