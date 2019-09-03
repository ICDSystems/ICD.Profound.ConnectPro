using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.CUE;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.CUE
{
	[ViewBinding(typeof(ISettingsCueBackgroundView))]
	public sealed partial class SettingsCueBackgroundView : AbstractUiView, ISettingsCueBackgroundView
	{
		private const ushort INDEX_STATIC = 0;
		private const ushort INDEX_SEASONAL = 1;

		/// <summary>
		/// Raised when the user presses the toggle button.
		/// </summary>
		public event EventHandler OnTogglePressed;

		/// <summary>
		/// Raised when the user presses the static button.
		/// </summary>
		public event EventHandler OnStaticButtonPressed;

		/// <summary>
		/// Raised when the user presses the seasonal button.
		/// </summary>
		public event EventHandler OnSeasonalButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsCueBackgroundView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnTogglePressed = null;
			OnStaticButtonPressed = null;
			OnSeasonalButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the selection state of the static button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetStaticButtonSelected(bool selected)
		{
			m_ModeButtons.SetItemSelected(INDEX_STATIC, selected);
		}

		/// <summary>
		/// Sets the selection state of the seasonal button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetSeasonalButtonSelection(bool selected)
		{
			m_ModeButtons.SetItemSelected(INDEX_SEASONAL, selected);
		}

		/// <summary>
		/// Sets the toggle state of the toggle button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetToggleSelected(bool selected)
		{
			// Enable is default
			m_EnabledToggleButton.SetSelected(!selected);
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ModeButtons.OnButtonPressed += ModeButtonsOnButtonPressed;
			m_EnabledToggleButton.OnPressed += EnabledToggleButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ModeButtons.OnButtonPressed -= ModeButtonsOnButtonPressed;
			m_EnabledToggleButton.OnPressed -= EnabledToggleButtonOnPressed;
		}

		private void ModeButtonsOnButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case INDEX_STATIC:
					OnStaticButtonPressed.Raise(this);
					break;

				case INDEX_SEASONAL:
					OnSeasonalButtonPressed.Raise(this);
					break;
			}
		}

		private void EnabledToggleButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnTogglePressed.Raise(this);
		}

		#endregion
	}
}
