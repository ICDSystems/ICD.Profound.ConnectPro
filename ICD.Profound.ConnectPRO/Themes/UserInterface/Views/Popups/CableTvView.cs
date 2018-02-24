using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups
{
	public sealed partial class CableTvView : AbstractPopupView, ICableTvView
	{
		public override event EventHandler OnCloseButtonPressed;

		public event EventHandler<UShortEventArgs> OnMenuButtonPressed; 
		public event EventHandler<CharEventArgs> OnNumberButtonPressed;
		public event EventHandler OnClearButtonPressed;
		public event EventHandler OnEnterButtonPressed;

		public event EventHandler OnUpButtonPressed;
		public event EventHandler OnDownButtonPressed;
		public event EventHandler OnLeftButtonPressed;
		public event EventHandler OnRightButtonPressed;
		public event EventHandler OnSelectButtonPressed;

		public event EventHandler OnChannelUpButtonPressed;
		public event EventHandler OnChannelDownButtonPressed;
		public event EventHandler OnPageUpButtonPressed;
		public event EventHandler OnPageDownButtonPressed;

		public event EventHandler OnRepeatButtonPressed;
		public event EventHandler OnRewindButtonPressed;
		public event EventHandler OnFastForwardButtonPressed;
		public event EventHandler OnStopButtonPressed;
		public event EventHandler OnPlayButtonPressed;
		public event EventHandler OnPauseButtonPressed;
		public event EventHandler OnRecordButtonPressed;

		public event EventHandler OnRedButtonPressed;
		public event EventHandler OnGreenButtonPressed;
		public event EventHandler OnBlueButtonPressed;
		public event EventHandler OnYellowButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public CableTvView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnCloseButtonPressed = null;

			OnMenuButtonPressed = null;
			OnNumberButtonPressed = null;
			OnClearButtonPressed = null;
			OnEnterButtonPressed = null;

			OnUpButtonPressed = null;
			OnDownButtonPressed = null;
			OnLeftButtonPressed = null;
			OnRightButtonPressed = null;
			OnSelectButtonPressed = null;

			OnChannelUpButtonPressed = null;
			OnChannelDownButtonPressed = null;
			OnPageUpButtonPressed = null;
			OnPageDownButtonPressed = null;

			OnRepeatButtonPressed = null;
			OnRewindButtonPressed = null;
			OnFastForwardButtonPressed = null;
			OnStopButtonPressed = null;
			OnPlayButtonPressed = null;
			OnPauseButtonPressed = null;
			OnRecordButtonPressed = null;

			OnRedButtonPressed = null;
			OnGreenButtonPressed = null;
			OnBlueButtonPressed = null;
			OnYellowButtonPressed = null;

			base.Dispose();
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_CloseButton.OnPressed += CloseButtonOnPressed;

			m_MenuButtons.OnButtonPressed += MenuButtonsOnButtonPressed;
			m_MenuDirections.OnButtonPressed += MenuDirectionsOnButtonPressed;
			m_NumberKeypad.OnButtonPressed += NumberKeypadOnButtonPressed;

			m_ChannelUpButton.OnPressed += ChannelUpButtonOnPressed;
			m_ChannelDownButton.OnPressed += ChannelDownButtonOnPressed;
			m_PageUpButton.OnPressed += PageUpButtonOnPressed;
			m_PageDownButton.OnPressed += PageDownButtonOnPressed;

			m_RedButton.OnPressed += RedButtonOnPressed;
			m_YellowButton.OnPressed += YellowButtonOnPressed;
			m_GreenButton.OnPressed += GreenButtonOnPressed;
			m_BlueButton.OnPressed += BlueButtonOnPressed;

			m_RepeatButton.OnPressed += RepeatButtonOnPressed;
			m_RewindButton.OnPressed += RewindButtonOnPressed;
			m_StopButton.OnPressed += StopButtonOnPressed;
			m_PlayButton.OnPressed += PlayButtonOnPressed;
			m_PauseButton.OnPressed += PauseButtonOnPressed;
			m_FastForwardButton.OnPressed += FastForwardButtonOnPressed;
			m_RecordButton.OnPressed += RecordButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;

			m_MenuButtons.OnButtonPressed -= MenuButtonsOnButtonPressed;
			m_MenuDirections.OnButtonPressed -= MenuDirectionsOnButtonPressed;
			m_NumberKeypad.OnButtonPressed -= NumberKeypadOnButtonPressed;

			m_ChannelUpButton.OnPressed -= ChannelUpButtonOnPressed;
			m_ChannelDownButton.OnPressed -= ChannelDownButtonOnPressed;
			m_PageUpButton.OnPressed -= PageUpButtonOnPressed;
			m_PageDownButton.OnPressed -= PageDownButtonOnPressed;

			m_RedButton.OnPressed -= RedButtonOnPressed;
			m_YellowButton.OnPressed -= YellowButtonOnPressed;
			m_GreenButton.OnPressed -= GreenButtonOnPressed;
			m_BlueButton.OnPressed -= BlueButtonOnPressed;

			m_RepeatButton.OnPressed -= RepeatButtonOnPressed;
			m_RewindButton.OnPressed -= RewindButtonOnPressed;
			m_StopButton.OnPressed -= StopButtonOnPressed;
			m_PlayButton.OnPressed -= PlayButtonOnPressed;
			m_PauseButton.OnPressed -= PauseButtonOnPressed;
			m_FastForwardButton.OnPressed -= FastForwardButtonOnPressed;
			m_RecordButton.OnPressed -= RecordButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the close button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void CloseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnCloseButtonPressed.Raise(this);
		}

		private void MenuButtonsOnButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			OnMenuButtonPressed.Raise(this, new UShortEventArgs(eventArgs.Data));
		}

		private void MenuDirectionsOnButtonPressed(object sender, DPadEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case DPadEventArgs.eDirection.Up:
					OnUpButtonPressed.Raise(this);
					break;
				
				case DPadEventArgs.eDirection.Down:
					OnDownButtonPressed.Raise(this);
					break;
				
				case DPadEventArgs.eDirection.Left:
					OnLeftButtonPressed.Raise(this);
					break;
				
				case DPadEventArgs.eDirection.Right:
					OnRightButtonPressed.Raise(this);
					break;
				
				case DPadEventArgs.eDirection.Center:
					OnSelectButtonPressed.Raise(this);
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void NumberKeypadOnButtonPressed(object sender, SimpleKeypadEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case SimpleKeypadEventArgs.eButton.Zero:
				case SimpleKeypadEventArgs.eButton.One:
				case SimpleKeypadEventArgs.eButton.Two:
				case SimpleKeypadEventArgs.eButton.Three:
				case SimpleKeypadEventArgs.eButton.Four:
				case SimpleKeypadEventArgs.eButton.Five:
				case SimpleKeypadEventArgs.eButton.Six:
				case SimpleKeypadEventArgs.eButton.Seven:
				case SimpleKeypadEventArgs.eButton.Eight:
				case SimpleKeypadEventArgs.eButton.Nine:
					char value = m_NumberKeypad.GetButtonChar(eventArgs.Data);
					OnNumberButtonPressed.Raise(this, new CharEventArgs(value));
					break;

				case SimpleKeypadEventArgs.eButton.MiscButtonOne:
					OnClearButtonPressed.Raise(this);
					break;
				
				case SimpleKeypadEventArgs.eButton.MiscButtonTwo:
					OnEnterButtonPressed.Raise(this);
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ChannelUpButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnChannelUpButtonPressed.Raise(this);
		}

		private void ChannelDownButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnChannelDownButtonPressed.Raise(this);
		}

		private void PageUpButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnPageUpButtonPressed.Raise(this);
		}

		private void PageDownButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnPageDownButtonPressed.Raise(this);
		}

		private void RedButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnRedButtonPressed.Raise(this);
		}

		private void YellowButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnYellowButtonPressed.Raise(this);
		}

		private void GreenButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnGreenButtonPressed.Raise(this);
		}

		private void BlueButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnBlueButtonPressed.Raise(this);
		}

		private void RepeatButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnRepeatButtonPressed.Raise(this);
		}

		private void RewindButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnRewindButtonPressed.Raise(this);
		}

		private void StopButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnStopButtonPressed.Raise(this);
		}

		private void PlayButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnPlayButtonPressed.Raise(this);
		}

		private void PauseButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnPauseButtonPressed.Raise(this);
		}

		private void FastForwardButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnFastForwardButtonPressed.Raise(this);
		}

		private void RecordButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnRecordButtonPressed.Raise(this);
		}

		#endregion
	}
}
