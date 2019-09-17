using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views
{
	[ViewBinding(typeof(IGenericAlertView))]
	public sealed partial class GenericAlertView : AbstractTouchDisplayView, IGenericAlertView
	{
		public event EventHandler OnDismissButtonPressed;

		public GenericAlertView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		#region Methods

		public void SetAlertText(string text)
		{
			m_AlertMessageLabel.SetLabelText(text);
		}

		public void SetDismissButtonEnabled(bool enable)
		{
			m_DismissButton.Enable(enable);
		}

		#endregion

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_DismissButton.OnPressed += DismissButtonOnPressed;
		}

		private void DismissButtonOnPressed(object sender, EventArgs e)
		{
			OnDismissButtonPressed.Raise(this);
		}

		#endregion
	}
}