using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups
{
	public sealed partial class CableTvView : AbstractPopupView, ICableTvView
	{
		public override event EventHandler OnCloseButtonPressed;

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

		public event EventHandler OnTopMenuButtonPressed;
		public event EventHandler OnPopupMenuButtonPressed;
		public event EventHandler OnReturnButtonPressed;
		public event EventHandler OnInfoButtonPressed;
		public event EventHandler OnEjectButtonPressed;
		public event EventHandler OnPowerButtonPressed;

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

			OnTopMenuButtonPressed = null;
			OnPopupMenuButtonPressed = null;
			OnReturnButtonPressed = null;
			OnInfoButtonPressed = null;
			OnEjectButtonPressed = null;
			OnPowerButtonPressed = null;

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
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_CloseButton.OnPressed -= CloseButtonOnPressed;
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

		#endregion
	}
}
