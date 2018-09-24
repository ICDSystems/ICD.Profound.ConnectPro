using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Conferencing.ConferenceManagers;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Conferencing.EventArguments;
using ICD.Connect.Devices;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Core;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	public sealed class SettingsBasePresenter : AbstractPopupPresenter<ISettingsBaseView>, ISettingsBasePresenter
	{
		private const ushort SYSTEM_POWER = 0;
		private const ushort PASSCODE_SETTINGS = 1;
		private const ushort DIRECTORY = 2;

		private static readonly Dictionary<ushort, string> s_ButtonLabels =
			new Dictionary<ushort, string>
			{
				{SYSTEM_POWER, "System Power Preference"},
				{PASSCODE_SETTINGS, "Passcode Settings"},
				{DIRECTORY, "Directory"}
			};

		private static readonly Dictionary<ushort, Type> s_NavTypes = new Dictionary<ushort, Type>
			{
				{SYSTEM_POWER, typeof(ISettingsSystemPowerPresenter)},
				{PASSCODE_SETTINGS, typeof(ISettingsPasscodePresenter)},
				{DIRECTORY, typeof(ISettingsDirectoryPresenter)}
			};

		private readonly Dictionary<ushort, IPresenter> m_NavPages;
		private readonly SafeCriticalSection m_RefreshSection;

		private bool HasDirectoryControl { get { return Room != null && Room.GetControlRecursive<IDirectoryControl>() != null; } }

		private IPresenter m_Visible;

		public IPresenter Visible
		{
			get { return m_Visible; }
			set
			{
				if (value == m_Visible)
					return;

				m_Visible = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsBasePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_NavPages = new Dictionary<ushort, IPresenter>();
			foreach (KeyValuePair<ushort, Type> kvp in s_NavTypes)
				m_NavPages.Add(kvp.Key, nav.LazyLoadPresenter(kvp.Value));

			SubscribePages();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			UnsubscribePages();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsBaseView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				IEnumerable<string> labels = s_ButtonLabels.OrderValuesByKey();
				view.SetButtonLabels(labels);

				foreach (KeyValuePair<ushort, IPresenter> kvp in m_NavPages)
				{
					view.SetItemSelected(kvp.Key, kvp.Value.IsViewVisible);
					bool showButton = true;
					if (kvp.Key == DIRECTORY)
					{
						showButton = HasDirectoryControl;
					}
					view.SetButtonVisible(kvp.Key, showButton);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private void ShowMenu(ushort index)
		{
			m_NavPages[index].ShowView(true);
		}

		#region Page Callbacks

		/// <summary>
		/// Subscribe to the child page events.
		/// </summary>
		private void SubscribePages()
		{
			foreach (IPresenter presenter in m_NavPages.Values)
				presenter.OnViewVisibilityChanged += PresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the child page events.
		/// </summary>
		private void UnsubscribePages()
		{
			foreach (IPresenter presenter in m_NavPages.Values)
				presenter.OnViewVisibilityChanged -= PresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when a child page visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void PresenterOnViewVisibilityChanged(object sender, BoolEventArgs boolEventArgs)
		{
			if (boolEventArgs.Data)
				Visible = sender as IPresenter;
			else if (Visible == sender)
				Visible = null;

			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsBaseView view)
		{
			base.Subscribe(view);

			view.OnListItemPressed += ViewOnListItemPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsBaseView view)
		{
			base.Unsubscribe(view);

			view.OnListItemPressed -= ViewOnListItemPressed;
		}

		/// <summary>
		/// Called when the user presses a list item button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ViewOnListItemPressed(object sender, UShortEventArgs eventArgs)
		{
			ShowMenu(eventArgs.Data);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			if (args.Data)
			{
				ShowMenu(SYSTEM_POWER);
			}
			else
			{
				foreach (IPresenter presenter in m_NavPages.Values)
					presenter.ShowView(false);

				ICoreSettings settings = Room == null ? null : Room.Core.CopySettings();
				if (settings != null)
					FileOperations.SaveSettings(settings, true);
			}
		}

		#endregion
	}
}
