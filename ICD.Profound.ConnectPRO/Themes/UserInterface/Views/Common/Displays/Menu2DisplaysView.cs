using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Displays
{
	[ViewBinding(typeof(IMenu2DisplaysView))]
	public sealed partial class Menu2DisplaysView : AbstractUiView, IMenu2DisplaysView
	{
		private const ushort MODE_WHITE = 0;
		private const ushort MODE_GREY = 1;
		private const ushort MODE_YELLOW = 2;
		private const ushort MODE_GREEN = 3;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public Menu2DisplaysView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnDisplay1ButtonPressed = null;
			OnDisplay1SpeakerButtonPressed = null;
			OnDisplay2ButtonPressed = null;
			OnDisplay2SpeakerButtonPressed = null;

			base.Dispose();
		}

		#region Display 1

		public event EventHandler OnDisplay1ButtonPressed;
		public event EventHandler OnDisplay1SpeakerButtonPressed;

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		public void SetDisplay1Color(eDisplayColor color)
		{
			switch (color)
			{
				case eDisplayColor.White:
					m_Display1BackgroundButton.SetMode(MODE_WHITE);
					break;
				case eDisplayColor.Grey:
					m_Display1BackgroundButton.SetMode(MODE_GREY);
					break;
				case eDisplayColor.Yellow:
					m_Display1BackgroundButton.SetMode(MODE_YELLOW);
					break;
				case eDisplayColor.Green:
					m_Display1BackgroundButton.SetMode(MODE_GREEN);
					break;
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		public void SetDisplay1Icon(string icon)
		{
			m_Display1Icon.SetIcon(icon);
		}

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		public void SetDisplay1Line1Text(string text)
		{
			m_Display1Line1Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		public void SetDisplay1Line2Text(string text)
		{
			m_Display1Line2Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the visibility of the speaker button.
		/// </summary>
		/// <param name="visible"></param>
		public void ShowDisplay1SpeakerButton(bool visible)
		{
			m_Display1SpeakerButton.Show(visible);
		}

		/// <summary>
		/// Sets the activity state of the speaker button.
		/// </summary>
		/// <param name="active"></param>
		public void SetDisplay1SpeakerButtonActive(bool active)
		{
			m_Display1SpeakerButton.SetSelected(!active);
		}

		/// <summary>
		/// Sets the text for the source label.
		/// </summary>
		/// <param name="text"></param>
		public void SetDisplay1SourceText(string text)
		{
			m_Display1SourceLabel.SetLabelText(text);
		}

		/// <summary>
		/// Warming/cooling bar graph - show/hide and set position
		/// </summary>
		/// <param name="visible"></param>
		/// <param name="position"></param>
		/// <param name="text"></param>
		public void SetDisplay1StatusGauge(bool visible, ushort position, string text)
		{
			m_Display1StatusGauge.Show(visible);
			m_Display1StatusGauge.SetValue(position);
			m_Display1StatusGauge.SetCenterChildLabel(text);
		}

		#endregion

		#region Display 2

		public event EventHandler OnDisplay2ButtonPressed;
		public event EventHandler OnDisplay2SpeakerButtonPressed;

		/// <summary>
		/// Sets the color style of the entire view.
		/// </summary>
		/// <param name="color"></param>
		public void SetDisplay2Color(eDisplayColor color)
		{
			switch (color)
			{
				case eDisplayColor.White:
					m_Display2BackgroundButton.SetMode(MODE_WHITE);
					break;
				case eDisplayColor.Grey:
					m_Display2BackgroundButton.SetMode(MODE_GREY);
					break;
				case eDisplayColor.Yellow:
					m_Display2BackgroundButton.SetMode(MODE_YELLOW);
					break;
				case eDisplayColor.Green:
					m_Display2BackgroundButton.SetMode(MODE_GREEN);
					break;
				default:
					throw new ArgumentOutOfRangeException("color");
			}
		}

		/// <summary>
		/// Sets the icon for the button.
		/// </summary>
		/// <param name="icon"></param>
		public void SetDisplay2Icon(string icon)
		{
			m_Display2Icon.SetIcon(icon);
		}

		/// <summary>
		/// Sets the text for the first label.
		/// </summary>
		/// <param name="text"></param>
		public void SetDisplay2Line1Text(string text)
		{
			m_Display2Line1Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the text for the second label.
		/// </summary>
		/// <param name="text"></param>
		public void SetDisplay2Line2Text(string text)
		{
			m_Display2Line2Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets the visibility of the speaker button.
		/// </summary>
		/// <param name="visible"></param>
		public void ShowDisplay2SpeakerButton(bool visible)
		{
			m_Display2SpeakerButton.Show(visible);
		}

		/// <summary>
		/// Sets the activity state of the speaker button.
		/// </summary>
		/// <param name="active"></param>
		public void SetDisplay2SpeakerButtonActive(bool active)
		{
			m_Display2SpeakerButton.SetSelected(!active);
		}

		/// <summary>
		/// Sets the text for the source label.
		/// </summary>
		/// <param name="text"></param>
		public void SetDisplay2SourceText(string text)
		{
			m_Display2SourceLabel.SetLabelText(text);
		}

		/// <summary>
		/// Warming/cooling bar graph - show/hide and set position
		/// </summary>
		/// <param name="visible"></param>
		/// <param name="position"></param>
		/// <param name="text"></param>
		public void SetDisplay2StatusGauge(bool visible, ushort position, string text)
		{
			m_Display2StatusGauge.Show(visible);
			m_Display2StatusGauge.SetValue(position);
			m_Display2StatusGauge.SetCenterLabel(text);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_Display1BackgroundButton.OnPressed += Display1BackgroundButtonOnPressed;
			m_Display1SpeakerButton.OnPressed += Display1SpeakerButtonOnPressed;
			m_Display2BackgroundButton.OnPressed += Display2BackgroundButtonOnPressed;
			m_Display2SpeakerButton.OnPressed += Display2SpeakerButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_Display1BackgroundButton.OnPressed -= Display1BackgroundButtonOnPressed;
			m_Display1SpeakerButton.OnPressed -= Display1SpeakerButtonOnPressed;
			m_Display2BackgroundButton.OnPressed -= Display2BackgroundButtonOnPressed;
			m_Display2SpeakerButton.OnPressed -= Display2SpeakerButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void Display1BackgroundButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDisplay1ButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void Display1SpeakerButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDisplay1SpeakerButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void Display2BackgroundButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDisplay2ButtonPressed.Raise(this);
		}

		/// <summary>
		/// Called when the user presses the speaker button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void Display2SpeakerButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnDisplay2SpeakerButtonPressed.Raise(this);
		}

		#endregion
	}
}
