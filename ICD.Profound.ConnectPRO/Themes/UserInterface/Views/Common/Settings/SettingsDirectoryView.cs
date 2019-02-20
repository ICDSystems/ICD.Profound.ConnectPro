using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	[ViewBinding(typeof(ISettingsDirectoryView))]
	public sealed partial class SettingsDirectoryView : AbstractUiView, ISettingsDirectoryView
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
