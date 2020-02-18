using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Zoom;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.Zoom
{
	[ViewBinding(typeof(ISettingsZoomView))]
	public sealed partial class SettingsZoomView : AbstractUiView, ISettingsZoomView
	{
		private const ushort INDEX_GENERAL = 0;
		private const ushort INDEX_ADVANCED = 1;
        private const ushort INDEX_CAMERAS = 2;

		/// <summary>
		/// Raised when the user presses the general button.
		/// </summary>
		public event EventHandler OnGeneralButtonPressed;

		/// <summary>
		/// Raised when the user presses the advanced button.
		/// </summary>
		public event EventHandler OnAdvancedButtonPressed;

		/// <summary>
		/// Raised when the user presses the cameras button.
		/// </summary>
        public event EventHandler OnCamerasButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsZoomView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnGeneralButtonPressed = null;
			OnAdvancedButtonPressed = null;
            OnCamerasButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sets the selection state of the general button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetGeneralButtonSelected(bool selected)
		{
			m_TabButtons.SetItemSelected(INDEX_GENERAL, selected);
		}

		/// <summary>
		/// Sets the selection state of the advanced button.
		/// </summary>
		/// <param name="selected"></param>
		public void SetAdvancedButtonSelection(bool selected)
		{
			m_TabButtons.SetItemSelected(INDEX_ADVANCED, selected);
		}

        /// <summary>
        /// Sets the selection state of the cameras button.
        /// </summary>
        /// <param name="selected"></param>
        public void SetCamerasButtonSelection(bool selected)
        {
			m_TabButtons.SetItemSelected(INDEX_CAMERAS, selected);
        }

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_TabButtons.OnButtonPressed += TabButtonsOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_TabButtons.OnButtonPressed -= TabButtonsOnButtonPressed;
		}

		/// <summary>
		/// Called when the user presses one of the tab buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void TabButtonsOnButtonPressed(object sender, UShortEventArgs eventArgs)
		{
			switch (eventArgs.Data)
			{
				case INDEX_GENERAL:
					OnGeneralButtonPressed.Raise(this);
					break;

				case INDEX_ADVANCED:
					OnAdvancedButtonPressed.Raise(this);
					break;

				case INDEX_CAMERAS:
					OnCamerasButtonPressed.Raise(this);
                    break;
			}
		}

		#endregion
	}
}
