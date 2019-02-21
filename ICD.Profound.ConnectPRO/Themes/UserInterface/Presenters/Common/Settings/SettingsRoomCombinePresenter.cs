using System;
using ICD.Common.Utils;
using ICD.Connect.Partitioning.Cells;
using ICD.Connect.Partitioning.Controls;
using ICD.Connect.Partitioning.PartitionManagers;
using ICD.Connect.Partitioning.Partitions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings
{
	[PresenterBinding(typeof(ISettingsRoomCombinePresenter))]
	public sealed class SettingsRoomCombinePresenter : AbstractUiPresenter<ISettingsRoomCombineView>, ISettingsRoomCombinePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private IPartitionManager m_SubscribedPartitionManager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public SettingsRoomCombinePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(ISettingsRoomCombineView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				for (int row = 0; row < 4; row++)
				{
					for (int column = 0; column < 4; column++)
					{
						ICell cell = m_SubscribedPartitionManager == null
							? null
							: m_SubscribedPartitionManager.Cells.GetCell(column, row);
						IRoom room = cell == null ? null : cell.Room;

						view.SetCellVisible(column, row, room != null);
						view.SetCellLabel(column, row, room == null ? null : room.Name);

						bool topVisible = GetWallVisible(column, row, ePartitionDirection.Top);
						bool topEnabled = GetWallEnabled(column, row, ePartitionDirection.Top);
						bool topSelected = GetWallSelected(column, row, ePartitionDirection.Top);

						view.SetWallVisible(column, row, ePartitionDirection.Top, topVisible);
						view.SetWallEnabled(column, row, ePartitionDirection.Top, topEnabled);
						view.SetWallSelected(column, row, ePartitionDirection.Top, topSelected);

						bool leftVisible = GetWallVisible(column, row, ePartitionDirection.Left);
						bool leftEnabled = GetWallEnabled(column, row, ePartitionDirection.Left);
						bool leftSelected = GetWallSelected(column, row, ePartitionDirection.Left);

						view.SetWallVisible(column, row, ePartitionDirection.Left, leftVisible);
						view.SetWallEnabled(column, row, ePartitionDirection.Left, leftEnabled);
						view.SetWallSelected(column, row, ePartitionDirection.Left, leftSelected);

						if (column == 3)
						{
							bool rightVisible = GetWallVisible(column, row, ePartitionDirection.Right);
							bool rightEnabled = GetWallEnabled(column, row, ePartitionDirection.Right);
							bool rightSelected = GetWallSelected(column, row, ePartitionDirection.Right);

							view.SetWallVisible(column, row, ePartitionDirection.Right, rightVisible);
							view.SetWallEnabled(column, row, ePartitionDirection.Right, rightEnabled);
							view.SetWallSelected(column, row, ePartitionDirection.Right, rightSelected);
						}

						if (row == 3)
						{
							bool bottomVisible = GetWallVisible(column, row, ePartitionDirection.Bottom);
							bool bottomEnabled = GetWallEnabled(column, row, ePartitionDirection.Bottom);
							bool bottomSelected = GetWallSelected(column, row, ePartitionDirection.Bottom);

							view.SetWallVisible(column, row, ePartitionDirection.Bottom, bottomVisible);
							view.SetWallEnabled(column, row, ePartitionDirection.Bottom, bottomEnabled);
							view.SetWallSelected(column, row, ePartitionDirection.Bottom, bottomSelected);
						}
					}
				}

				view.SetClearAllButtonEnabled(false);
				view.SetSaveButtonEnabled(false);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private bool GetWallVisible(int column, int row, ePartitionDirection direction)
		{
			throw new NotImplementedException();
		}

		private bool GetWallEnabled(int column, int row, ePartitionDirection direction)
		{
			throw new NotImplementedException();
		}

		private bool GetWallSelected(int column, int row, ePartitionDirection direction)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the room for this presenter to represent.
		/// </summary>
		/// <param name="room"></param>
		public override void SetRoom(IConnectProRoom room)
		{
			base.SetRoom(room);

			Unsubscribe(m_SubscribedPartitionManager);
			m_SubscribedPartitionManager = room == null ? null : room.Core.Originators.GetChild<IPartitionManager>();
			Subscribe(m_SubscribedPartitionManager);
		}

		#region Partition Manager Callbacks

		private void Subscribe(IPartitionManager partitionManager)
		{
			if (partitionManager == null)
				return;

			partitionManager.OnPartitionOpenStateChange += PartitionManagerOnPartitionOpenStateChange;
		}

		private void Unsubscribe(IPartitionManager partitionManager)
		{
			if (partitionManager == null)
				return;

			partitionManager.OnPartitionOpenStateChange -= PartitionManagerOnPartitionOpenStateChange;
		}

		private void PartitionManagerOnPartitionOpenStateChange(IPartitionDeviceControl control, bool open)
		{
			RefreshIfVisible();
		}

		#endregion

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(ISettingsRoomCombineView view)
		{
			base.Subscribe(view);

			view.OnClearAllButtonPressed += ViewOnClearAllButtonPressed;
			view.OnSaveButtonPressed += ViewOnSaveButtonPressed;
			view.OnWallButtonPressed += ViewOnWallButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(ISettingsRoomCombineView view)
		{
			base.Unsubscribe(view);

			view.OnClearAllButtonPressed -= ViewOnClearAllButtonPressed;
			view.OnSaveButtonPressed -= ViewOnSaveButtonPressed;
			view.OnWallButtonPressed -= ViewOnWallButtonPressed;
		}

		private void ViewOnSaveButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		private void ViewOnClearAllButtonPressed(object sender, EventArgs eventArgs)
		{
		}

		private void ViewOnWallButtonPressed(object sender, PartitionDirectionEventArgs eventArgs)
		{
			IcdConsole.PrintLine(eConsoleColor.Magenta, "Partition ({0}, {1}) {2} Pressed", eventArgs.Column, eventArgs.Row, eventArgs.Direction);
		}

		#endregion
	}
}
