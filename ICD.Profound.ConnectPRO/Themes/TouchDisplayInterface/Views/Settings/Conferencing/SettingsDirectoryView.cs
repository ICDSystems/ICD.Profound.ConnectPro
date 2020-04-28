using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.Conferencing;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Settings.Conferencing
{
	[ViewBinding(typeof(ISettingsDirectoryView))]
	public sealed partial class SettingsDirectoryView : AbstractTouchDisplayView, ISettingsDirectoryView
	{
		/// <summary>
		/// Raised when the user presses the clear directory button.
		/// </summary>
		public event EventHandler OnClearDirectoryButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsDirectoryView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			OnClearDirectoryButtonPressed = null;
		}

		/// <summary>
		/// Sets the text for the help label.
		/// </summary>
		/// <param name="text"></param>
		public void SetHelpText(string text)
		{
			m_HelpLabel.SetLabelText(text);
		}

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_ClearCacheButton.OnPressed += ClearCacheButtonOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_ClearCacheButton.OnPressed -= ClearCacheButtonOnPressed;
		}

		/// <summary>
		/// Called when the user presses the clear cache button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ClearCacheButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnClearDirectoryButtonPressed.Raise(this);
		}

		#endregion
	}
}
