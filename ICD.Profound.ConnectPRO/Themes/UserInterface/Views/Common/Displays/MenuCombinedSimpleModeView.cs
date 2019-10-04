using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	[ViewBinding(typeof(IMenuCombinedSimpleModeView))]
	public sealed partial class MenuCombinedSimpleModeView : AbstractUiView, IMenuCombinedSimpleModeView
	{
		private const ushort MODE_GREY = 0;
		private const ushort MODE_WHITE = 1;
		private const ushort MODE_YELLOW = 2;
		private const ushort MODE_GREEN = 3;

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

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		public void SetDisplayColor(eDisplayColor color)
		{
			switch (color)
			{
				case eDisplayColor.White:
					m_DisplayButton.SetMode(MODE_WHITE);
					break;
				case eDisplayColor.Grey:
					m_DisplayButton.SetMode(MODE_GREY);
					break;
				case eDisplayColor.Yellow:
					m_DisplayButton.SetMode(MODE_YELLOW);
					break;
				case eDisplayColor.Green:
					m_DisplayButton.SetMode(MODE_GREEN);
					break;
				default:
					throw new ArgumentOutOfRangeException("color");
			}
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

		/// <summary>
		/// Warming/cooling bar graph - show/hide and set position and text
		/// </summary>
		/// <param name="text"></param>
		public void SetDisplayWarmupStatusText(string text)
		{
			m_WarmupLabel.SetLabelText(text);
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