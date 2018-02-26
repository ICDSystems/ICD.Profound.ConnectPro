using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class StartMeetingView : AbstractView, IStartMeetingView
	{
		public event EventHandler OnStartMeetingButtonPressed;
		public event EventHandler OnShutdownButtonPressed;

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

		private void ShutdownButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnShutdownButtonPressed.Raise(this);
		}

		private void StartMeetingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnStartMeetingButtonPressed.Raise(this);
		}

		#endregion
	}
}
