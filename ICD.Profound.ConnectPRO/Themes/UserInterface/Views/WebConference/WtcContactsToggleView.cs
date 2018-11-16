using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference
{
	public sealed partial class WtcContactsToggleView : AbstractUiView, IWtcContactsToggleView
	{
		public event EventHandler OnButtonPressed;

		public WtcContactsToggleView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public override void Dispose()
		{
			base.Dispose();

			OnButtonPressed = null;
		}

		#region Methods

		public void SetContactsMode(bool mode)
		{
			m_ContactsButton.SetSelected(mode);
		}

		public void SetButtonVisible(bool visible)
		{
			m_ContactsButton.Show(visible);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ContactsButton.OnPressed += ContactsButtonOnOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ContactsButton.OnPressed -= ContactsButtonOnOnPressed;
		}

		private void ContactsButtonOnOnPressed(object sender, EventArgs eventArgs)
		{
			OnButtonPressed.Raise(this);
		}

		#endregion
	}
}