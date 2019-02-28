using System;
using System.Linq;
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
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings;

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
				for (int row = 0; row < SettingsRoomCombineView.ROWS; row++)
				{
					for (int column = 0; column < SettingsRoomCombineView.COLUMNS; column++)
					{
						ICell cell = m_SubscribedPartitionManager == null
							? null
							: m_SubscribedPartitionManager.Cells.GetCell(column, row);
						IRoom room = cell == null ? null : cell.Room;

						view.SetCellVisible(column, row, room != null);
						view.SetCellLabel(column, row, room == null ? null : room.Name);
						
						bool topEnabled = GetWallEnabled(column, row, eCellDirection.Top);
						bool topSelected = GetWallSelected(column, row, eCellDirection.Top);
						eWallButtonMode topMode = GetWallMode(column, row, eCellDirection.Top);
						
						view.SetWallEnabled(column, row, eCellDirection.Top, true);
						view.SetWallSelected(column, row, eCellDirection.Top, true);
						view.SetWallMode(column, row, eCellDirection.Top, topMode);
						
						bool leftEnabled = GetWallEnabled(column, row, eCellDirection.Left);
						bool leftSelected = GetWallSelected(column, row, eCellDirection.Left);
						eWallButtonMode leftMode = GetWallMode(column, row, eCellDirection.Left);

						view.SetWallEnabled(column, row, eCellDirection.Left, true);
						view.SetWallSelected(column, row, eCellDirection.Left, false);
						view.SetWallMode(column, row, eCellDirection.Left, leftMode);

						if (column == SettingsRoomCombineView.COLUMNS - 1)
						{
							bool rightEnabled = GetWallEnabled(column, row, eCellDirection.Right);
							bool rightSelected = GetWallSelected(column, row, eCellDirection.Right);
							eWallButtonMode rightMode = GetWallMode(column, row, eCellDirection.Right);
							
							view.SetWallEnabled(column, row, eCellDirection.Right, true);
							view.SetWallSelected(column, row, eCellDirection.Right, false);
							view.SetWallMode(column, row, eCellDirection.Right, rightMode);
						}

						if (row == SettingsRoomCombineView.ROWS - 1)
						{
							bool bottomEnabled = GetWallEnabled(column, row, eCellDirection.Bottom);
							bool bottomSelected = GetWallSelected(column, row, eCellDirection.Bottom);
							eWallButtonMode bottomMode = GetWallMode(column, row, eCellDirection.Bottom);
							
							view.SetWallEnabled(column, row, eCellDirection.Bottom, true);
							view.SetWallSelected(column, row, eCellDirection.Bottom, false);
							view.SetWallMode(column, row, eCellDirection.Bottom, bottomMode);
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

		private bool GetWallEnabled(int column, int row, eCellDirection direction)
		{
			return m_SubscribedPartitionManager.GetPartition(column, row, direction) != null;
		}

		private bool GetWallSelected(int column, int row, eCellDirection direction)
		{
			throw new NotImplementedException();
		}

		private eWallButtonMode GetWallMode(int column, int row, eCellDirection direction)
		{
			if (m_SubscribedPartitionManager == null)
				return eWallButtonMode.NoWall;

			var cell = m_SubscribedPartitionManager.Cells.GetCell(column, row);
			var neighboringCell = m_SubscribedPartitionManager.Cells.GetNeighboringCell(column, row, direction);

			var room = cell != null ? cell.Room : null;
			var neighboringRoom = neighboringCell != null ? neighboringCell.Room : null;
			if ((room == null && neighboringRoom == null) || room == neighboringRoom)
				return eWallButtonMode.NoWall;
			if (room == null || neighboringRoom == null)
				return eWallButtonMode.PermanentWall;

			var partition = m_SubscribedPartitionManager.GetPartition(column, row, direction);
			var controls = partition.GetPartitionControls()
			                        .Select(info => Room.GetControl(info) as IPartitionDeviceControl)
			                        .Where(c => c != null);
			return controls.All(c => c.IsOpen) ? eWallButtonMode.OpenPartition : eWallButtonMode.ClosedPartition;
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

		private void ViewOnWallButtonPressed(object sender, CellDirectionEventArgs eventArgs)
		{
			IcdConsole.PrintLine(eConsoleColor.Magenta, "Partition ({0}, {1}) {2} Pressed", eventArgs.Column, eventArgs.Row, eventArgs.Direction);
		}

		#endregion
	}
}
