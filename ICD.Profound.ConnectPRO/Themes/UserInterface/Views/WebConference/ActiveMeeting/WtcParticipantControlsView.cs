using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.ActiveMeeting;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.ActiveMeeting
{
	[ViewBinding(typeof(IWtcParticipantControlsView))]
	public sealed partial class WtcParticipantControlsView : AbstractUiView, IWtcParticipantControlsView
	{
		public event EventHandler<UShortEventArgs> OnButtonPressed;

		public WtcParticipantControlsView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetButtonIcon(ushort index, string icon)
		{
			m_ButtonList.SetItemIcon(index, icon);
		}

		public void SetButtonLabel(ushort index, string label)
		{
			m_ButtonList.SetItemLabel(index, label);
		}

		public void SetButtonVisible(ushort index, bool visible)
		{
			m_ButtonList.SetItemVisible(index, visible);
		}

		public void SetButtonEnabled(ushort index, bool enabled)
		{
			m_ButtonList.SetItemEnabled(index, enabled);
		}

		public void SetButtonSelected(ushort index, bool selected)
		{
			m_ButtonList.SetItemSelected(index, selected);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ButtonList.OnButtonClicked += ButtonListOnOnButtonClicked;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ButtonList.OnButtonClicked -= ButtonListOnOnButtonClicked;
		}

		private void ButtonListOnOnButtonClicked(object sender, UShortEventArgs e)
		{
			OnButtonPressed.Raise(this, new UShortEventArgs(e.Data));
		}

		#endregion
	}
}