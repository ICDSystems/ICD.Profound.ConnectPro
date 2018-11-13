using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public sealed partial class WtcActiveMeetingToggleView : AbstractView, IWtcActiveMeetingToggleView
	{
		public event EventHandler OnButtonPressed;

		public WtcActiveMeetingToggleView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public override void Dispose()
		{
			base.Dispose();

			OnButtonPressed = null;
		}

		#region Methods

		public void SetActiveMeetingMode(bool mode)
		{
			m_ActiveMeetingButton.SetSelected(mode);
		}

		public void SetButtonVisible(bool visible)
		{
			m_ActiveMeetingButton.Show(visible);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ActiveMeetingButton.OnPressed += ActiveMeetingButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ActiveMeetingButton.OnPressed -= ActiveMeetingButtonOnOnPressed;
		}

		private void ActiveMeetingButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}