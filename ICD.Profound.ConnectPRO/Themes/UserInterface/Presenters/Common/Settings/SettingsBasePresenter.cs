using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.Conferencing.Controls.Directory;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Cores;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	[PresenterBinding(typeof(ISettingsBasePresenter))]
	public sealed class SettingsBasePresenter : AbstractPopupPresenter<ISettingsBaseView>, ISettingsBasePresenter
	{
		private const ushort DIRECTORY = 0;
		private const ushort PASSCODE_SETTINGS = 1;
		private const ushort ROOM_COMBINE = 2;
		private const ushort SYSTEM_POWER = 3;

		private static readonly IcdOrderedDictionary<ushort, string> s_ButtonLabels =
			new IcdOrderedDictionary<ushort, string>
			{
				{DIRECTORY, "Directory"},
				{PASSCODE_SETTINGS, "Passcode Settings"},
				{ROOM_COMBINE, "Room Combine"},
				{SYSTEM_POWER, "System Power Preference"},
			};

		private static readonly Dictionary<ushort, Type> s_NavTypes =
			new Dictionary<ushort, Type>
			{
				{DIRECTORY, typeof(ISettingsDirectoryPresenter)},
				{PASSCODE_SETTINGS, typeof(ISettingsPasscodePresenter)},
				{ROOM_COMBINE, typeof(ISettingsRoomCombinePresenter)},
				{SYSTEM_POWER, typeof(ISettingsSystemPowerPresenter)},
			};

		private readonly Dictionary<ushort, IUiPresenter> m_NavPages;
		private readonly SafeCriticalSection m_RefreshSection;

		private bool m_HasDirectoryControl;
		private bool m_HasMultipleRooms;

		private IUiPresenter m_CurrentSettingsPage;

		public IUiPresenter CurrentSettingsPage
		{
			get { return m_CurrentSettingsPage; }
			set
			{
				if (value == m_CurrentSettingsPage)
					return;

				m_CurrentSettingsPage = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsBasePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();

			m_NavPages = new Dictionary<ushort, IUiPresenter>();
			foreach (KeyValuePair<ushort, Type> kvp in s_NavTypes)
				m_NavPages.Add(kvp.Key, nav.LazyLoadPresenter(kvp.Value) as IUiPresenter);

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
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			m_HasDirectoryControl = room != null && room.GetControlRecursive<IDirectoryControl>() != null;
			m_HasMultipleRooms = room != null && room.Core.Originators.GetChildren<IRoom>().Count() > 1;

			RefreshIfVisible();
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
				view.SetButtonLabels(s_ButtonLabels.Values);

				foreach (KeyValuePair<ushort, IUiPresenter> kvp in m_NavPages)
				{
					bool showButton = ShouldShowButton(kvp.Key);

					view.SetItemSelected(kvp.Key, kvp.Value.IsViewVisible);
					view.SetButtonVisible(kvp.Key, showButton);
				}
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region Private Methods

		/// <summary>
		/// Returns true if the button at the given index should be visible.
		/// </summary>
		/// <param name="buttonIndex"></param>
		/// <returns></returns>
		private bool ShouldShowButton(ushort buttonIndex)
		{
			switch (buttonIndex)
			{
				case DIRECTORY:
					return m_HasDirectoryControl;

				case ROOM_COMBINE:
					return m_HasMultipleRooms;

				default:
					return true;
			}
		}

		/// <summary>
		/// Sets the visibility of the settings page for the button at the given index.
		/// </summary>
		/// <param name="buttonIndex"></param>
		private void ShowSettingsPage(ushort buttonIndex)
		{
			m_NavPages[buttonIndex].ShowView(true);
		}

		#endregion

		#region Page Callbacks

		/// <summary>
		/// Subscribe to the child page events.
		/// </summary>
		private void SubscribePages()
		{
			foreach (IUiPresenter presenter in m_NavPages.Values)
				presenter.OnViewVisibilityChanged += PresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the child page events.
		/// </summary>
		private void UnsubscribePages()
		{
			foreach (IUiPresenter presenter in m_NavPages.Values)
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
				CurrentSettingsPage = sender as IUiPresenter;
			else if (CurrentSettingsPage == sender)
				CurrentSettingsPage = null;

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
			ShowSettingsPage(eventArgs.Data);
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
				ShowSettingsPage(SYSTEM_POWER);
			}
			else
			{
				foreach (IUiPresenter presenter in m_NavPages.Values)
					presenter.ShowView(false);

				ICoreSettings settings = Room == null ? null : Room.Core.CopySettings();
				if (settings != null)
					FileOperations.SaveSettings(settings, true);
			}
		}

		#endregion
	}
}
