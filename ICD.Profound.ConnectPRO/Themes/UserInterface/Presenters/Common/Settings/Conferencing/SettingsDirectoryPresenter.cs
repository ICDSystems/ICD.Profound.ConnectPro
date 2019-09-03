using System;
using System.Collections.Generic;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.Conferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.Conferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.Conferencing
{
	[PresenterBinding(typeof(ISettingsDirectoryPresenter))]
	public sealed class SettingsDirectoryPresenter : AbstractUiPresenter<ISettingsDirectoryView>, ISettingsDirectoryPresenter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsDirectoryPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
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
			
			IEnumerable<IDirectoryControl> directoryControls = Room.GetControlsRecursive<IDirectoryControl>();

			foreach (var directoryControl in directoryControls)
			{
				directoryControl.Clear();
				directoryControl.PopulateFolder(directoryControl.GetRoot());
			}
		}

		#endregion
	}
}
