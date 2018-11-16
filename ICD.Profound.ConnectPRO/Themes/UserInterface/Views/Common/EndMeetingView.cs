using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class EndMeetingView : AbstractUiView, IEndMeetingView
	{
		public event EventHandler OnEndMeetingButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public EndMeetingView(ISigInputOutput panel, ConnectProTheme theme)
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

			m_EndMeetingButton.OnPressed += EndMeetingButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_EndMeetingButton.OnPressed -= EndMeetingButtonOnPressed;
		}

		private void EndMeetingButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnEndMeetingButtonPressed.Raise(this);
		}

		#endregion
	}
}
