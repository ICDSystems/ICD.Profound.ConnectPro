using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Conference
{
	[ViewBinding(typeof(IConferenceBaseView))]
	public sealed partial class ConferenceBaseView : AbstractPopupView, IConferenceBaseView
	{
		public override event EventHandler OnCloseButtonPressed;

		public ConferenceBaseView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
		}

		private void CloseButtonOnPressed(object sender, EventArgs e)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}