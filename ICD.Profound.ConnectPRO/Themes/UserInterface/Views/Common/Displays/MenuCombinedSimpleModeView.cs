using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	[ViewBinding(typeof(IMenuCombinedSimpleModeView))]
	public sealed partial class MenuCombinedSimpleModeView : AbstractUiView, IMenuCombinedSimpleModeView
	{
		public event EventHandler OnAdvancedModeButtonPressed;
		public event EventHandler OnDisplayButtonPressed;
		public event EventHandler OnDisplaySpeakerButtonPressed;

		public MenuCombinedSimpleModeView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}
		
		public void SetAdvancedModeEnabled(bool enabled)
		{
			m_AdvancedModeButton.Enable(enabled);
		}

		public void SetDisplayIcon(string icon)
		{
			m_DisplayIcon.SetIcon(icon);
		}

		public void SetDisplayLine1Text(string text)
		{
			m_Line1Label.SetLabelText(text);
		}

		public void SetDisplayLine2Text(string text)
		{
			m_Line2Label.SetLabelText(text);
		}

		public void ShowDisplaySpeakerButton(bool visible)
		{
			m_SpeakerButton.Show(visible);
		}

		public void SetDisplaySpeakerButtonActive(bool active)
		{
			m_SpeakerButton.SetSelected(!active);
		}

		public void SetDisplaySourceText(string text)
		{
			m_SourceLabel.SetLabelText(text);
		}

		#region Control Callbacks

		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_SpeakerButton.OnPressed += SpeakerButtonOnPressed;
			m_DisplayButton.OnPressed += DisplayButtonOnPressed;
			m_AdvancedModeButton.OnPressed += AdvancedModeButtonOnPressed;
		}

		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_SpeakerButton.OnPressed -= SpeakerButtonOnPressed;
			m_DisplayButton.OnPressed -= DisplayButtonOnPressed;
			m_AdvancedModeButton.OnPressed -= AdvancedModeButtonOnPressed;
		}

		private void SpeakerButtonOnPressed(object sender, EventArgs args)
		{
			OnDisplaySpeakerButtonPressed.Raise(this);
		}

		private void DisplayButtonOnPressed(object sender, EventArgs args)
		{
			OnDisplayButtonPressed.Raise(this);
		}

		private void AdvancedModeButtonOnPressed(object sender, EventArgs args)
		{
			OnAdvancedModeButtonPressed.Raise(this);
		}

		#endregion
	}
}