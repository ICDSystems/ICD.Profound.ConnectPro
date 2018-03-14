using System;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.WebConferencing
{
	public sealed partial class WebConferencingStepView : AbstractView, IWebConferencingStepView
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
	}
}
