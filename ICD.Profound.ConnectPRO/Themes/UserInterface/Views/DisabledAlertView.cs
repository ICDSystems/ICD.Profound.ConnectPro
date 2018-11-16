using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	public sealed partial class DisabledAlertView : AbstractUiView, IDisabledAlertView
	{
		public event EventHandler OnDismissButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public DisabledAlertView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		#region Methods

		public override void Dispose()
		{
			OnDismissButtonPressed = null;

			base.Dispose();
		}

		#endregion

		#region Private Methods

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_dismissButton.OnPressed += DismissButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_dismissButton.OnPressed -= DismissButtonOnPressed;
		}

		private void DismissButtonOnPressed(object sender, EventArgs e)
		{
			OnDismissButtonPressed.Raise(this);
		}

		#endregion
	}
}