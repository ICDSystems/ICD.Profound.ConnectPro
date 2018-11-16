using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.WebConferencing
{
	public sealed partial class WebConferencingStepView : AbstractUiView, IWebConferencingStepView
	{
		public event EventHandler OnCloseButtonPressed;
		public event EventHandler OnBackButtonPressed;
		public event EventHandler OnForwardButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public WebConferencingStepView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCloseButtonPressed = null;
			OnBackButtonPressed = null;
			OnForwardButtonPressed = null;

			base.Dispose();
		}

		public void ShowBackButton(bool show)
		{
			m_BackButton.Show(show);
		}

		public void ShowForwardButton(bool show)
		{
			m_ForwardButton.Show(show);
		}

		public void SetImageUrl(string url)
		{
			m_InstructionImage.SetImageUrl(url);
		}

		public void SetText(string text)
		{
			m_InstructionLabel.SetLabelText(text);
		}

		public void SetStepNumber(ushort number)
		{
			m_StepLabel.SetLabelText(number.ToString());
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;
			m_BackButton.OnPressed += BackButtonOnPressed;
			m_ForwardButton.OnPressed += ForwardButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
			m_BackButton.OnPressed -= BackButtonOnPressed;
			m_ForwardButton.OnPressed -= ForwardButtonOnPressed;
		}

		private void ForwardButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnForwardButtonPressed.Raise(this);
		}

		private void BackButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnBackButtonPressed.Raise(this);
		}

		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		#endregion
	}
}
