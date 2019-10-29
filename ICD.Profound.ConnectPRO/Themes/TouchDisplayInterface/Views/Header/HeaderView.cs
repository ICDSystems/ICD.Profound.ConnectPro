using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Header
{
	[ViewBinding(typeof(IHeaderView))]
	public sealed partial class HeaderView : AbstractTouchDisplayView, IHeaderView
	{
		public event EventHandler OnCenterButtonPressed;

		public HeaderView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetRoomName(string name)
		{
			m_RoomName.SetLabelText(name);
		}

		public void SetTimeLabel(string time)
		{
			m_TimeLabel.SetLabelText(time);
		}

		public void SetCenterButtonIcon(string icon)
		{
			m_CenterButtonIcon.SetIcon(icon);
		}

		public void SetCenterButtonText(string text)
		{
			m_CenterButton.SetLabelText(text);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CenterButton.OnPressed += StartEndMeetingButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CenterButton.OnPressed -= StartEndMeetingButtonOnPressed;
		}

		private void StartEndMeetingButtonOnPressed(object sender, EventArgs e)
		{
			OnCenterButtonPressed.Raise(this);
		}

		#endregion
	}
}