using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	[ViewBinding(typeof(IWtcBaseView))]
	public sealed partial class WtcBaseView : AbstractPopupView, IWtcBaseView
	{
		public override event EventHandler OnCloseButtonPressed;

		public WtcBaseView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public override void Dispose()
		{
			OnCloseButtonPressed = null;

			base.Dispose();
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
		}

		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}