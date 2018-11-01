using System;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	public sealed class SettingsDirectoryPresenter : AbstractPresenter<ISettingsDirectoryView>, ISettingsDirectoryPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsDirectoryPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsDirectoryView view)
		{
			base.Subscribe(view);

			view.OnClearDirectoryButtonPressed += ViewOnClearDirectoryButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsDirectoryView view)
		{
			base.Unsubscribe(view);

			view.OnClearDirectoryButtonPressed -= ViewOnClearDirectoryButtonPressed;
		}

		/// <summary>
		/// Called when the user presses the clear directory button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnClearDirectoryButtonPressed(object sender, EventArgs eventArgs)
		{
			IConferenceManager manager = Room == null ? null : Room.ConferenceManager;
			IConferenceDeviceControl videoDialer = manager == null ? null : manager.GetDialingProvider(eCallType.Video);
			IDeviceBase parent = videoDialer == null ? null : videoDialer.Parent;
			IDirectoryControl directoryControl = parent == null ? null : parent.Controls.GetControl<IDirectoryControl>();

			if (directoryControl != null)
			{
				directoryControl.Clear();
				directoryControl.PopulateFolder(directoryControl.GetRoot());
			}
		}

		#endregion
	}
}
