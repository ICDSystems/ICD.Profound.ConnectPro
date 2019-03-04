using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Partitioning.Cells;
using ICD.Connect.Partitioning.Controls;
using ICD.Connect.Partitioning.PartitionManagers;
using ICD.Connect.Partitioning.Partitions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Rooms.Combine;
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
		private readonly SafeCriticalSection m_PartitionSection;
		private readonly Dictionary<IPartition, bool> m_SelectedPartitionStates;

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
			m_PartitionSection = new SafeCriticalSection();
			m_SelectedPartitionStates = new Dictionary<IPartition, bool>();
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
						view.SetCellSelected(column, row, room != null);
						view.SetCellEnabled(column, row, room != null);
						
						bool topEnabled = GetWallEnabled(column, row, eCellDirection.Top);
						bool topSelected = GetWallSelected(column, row, eCellDirection.Top);
						eWallButtonMode? topMode = GetWallMode(column, row, eCellDirection.Top);
						
						view.SetWallEnabled(column, row, eCellDirection.Top, topEnabled);
						view.SetWallSelected(column, row, eCellDirection.Top, topSelected);
						view.SetWallVisible(column, row, eCellDirection.Top, topMode != null);
						view.SetWallMode(column, row, eCellDirection.Top, topMode ?? eWallButtonMode.NoWall);
						
						bool leftEnabled = GetWallEnabled(column, row, eCellDirection.Left);
						bool leftSelected = GetWallSelected(column, row, eCellDirection.Left);
						eWallButtonMode? leftMode = GetWallMode(column, row, eCellDirection.Left);

						view.SetWallEnabled(column, row, eCellDirection.Left, leftEnabled);
						view.SetWallSelected(column, row, eCellDirection.Left, leftSelected);
						view.SetWallVisible(column, row, eCellDirection.Left, leftMode != null);
						view.SetWallMode(column, row, eCellDirection.Left, leftMode ?? eWallButtonMode.NoWall);

						if (column == SettingsRoomCombineView.COLUMNS - 1)
						{
							bool rightEnabled = GetWallEnabled(column, row, eCellDirection.Right);
							bool rightSelected = GetWallSelected(column, row, eCellDirection.Right);
							eWallButtonMode? rightMode = GetWallMode(column, row, eCellDirection.Right);
							
							view.SetWallEnabled(column, row, eCellDirection.Right, rightEnabled);
							view.SetWallSelected(column, row, eCellDirection.Right, rightSelected);
							view.SetWallVisible(column, row, eCellDirection.Right, rightMode != null);
							view.SetWallMode(column, row, eCellDirection.Right, rightMode ?? eWallButtonMode.NoWall);
						}

						if (row == SettingsRoomCombineView.ROWS - 1)
						{
							bool bottomEnabled = GetWallEnabled(column, row, eCellDirection.Bottom);
							bool bottomSelected = GetWallSelected(column, row, eCellDirection.Bottom);
							eWallButtonMode? bottomMode = GetWallMode(column, row, eCellDirection.Bottom);
							
							view.SetWallEnabled(column, row, eCellDirection.Bottom, bottomEnabled);
							view.SetWallSelected(column, row, eCellDirection.Bottom, bottomSelected);
							view.SetWallVisible(column, row, eCellDirection.Bottom, bottomMode != null);
							view.SetWallMode(column, row, eCellDirection.Bottom, bottomMode ?? eWallButtonMode.NoWall);
						}
					}
				}

				view.SetClearAllButtonEnabled(m_SelectedPartitionStates.Count > 0);
				view.SetSaveButtonEnabled(m_SelectedPartitionStates.Count > 0);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private bool GetWallEnabled(int column, int row, eCellDirection direction)
		{
			return true;
		}

		private bool GetWallSelected(int column, int row, eCellDirection direction)
		{
			return false;
		}

		private eWallButtonMode? GetWallMode(int column, int row, eCellDirection direction)
		{
			if (m_SubscribedPartitionManager == null)
				return null;

			var cell = m_SubscribedPartitionManager.Cells.GetCell(column, row);
			var neighboringCell = m_SubscribedPartitionManager.Cells.GetNeighboringCell(column, row, direction);
			var room = cell != null ? cell.Room : null;
			var neighboringRoom = neighboringCell != null ? neighboringCell.Room : null;

			// if both aren't rooms, then no wall
			if (room == null && neighboringRoom == null)
				return eWallButtonMode.NoWall;

			// if same room, don't show wall
			if (room == neighboringRoom)
				return null;

			// if room and no room, permanent wall
			if (room == null || neighboringRoom == null)
				return eWallButtonMode.PermanentWall;

			var partition = m_SubscribedPartitionManager.GetPartition(column, row, direction);
			// if there's no partition, permanent wall
			if (partition == null)
				return eWallButtonMode.PermanentWall;

			// if user has manually selected a partition state, show that
			m_PartitionSection.Enter();
			try
			{
				if (m_SelectedPartitionStates.ContainsKey(partition))
					return m_SelectedPartitionStates[partition] ? eWallButtonMode.UnsavedOpenPartition : eWallButtonMode.UnsavedClosedPartition;
			}
			finally
			{
				m_PartitionSection.Leave();
			}

			var controls = partition.GetPartitionControls()
			                        .Select(info => Room.Core.GetControl(info.Control) as IPartitionDeviceControl)
			                        .Where(c => c != null).ToList();
			// if there's no controls to get/set partition state, permanent wall
			if (!controls.Any())
				return eWallButtonMode.PermanentWall;

			// show state of partition
			return controls.Any(c => c.IsOpen) ? eWallButtonMode.OpenPartition : eWallButtonMode.ClosedPartition;
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
			var partitions = m_SubscribedPartitionManager.Partitions.GetPartitions(control).ToList();

			m_PartitionSection.Enter();
			try
			{
				foreach (var partition in partitions)
				{
					if (m_SelectedPartitionStates.ContainsKey(partition) && m_SelectedPartitionStates[partition] == control.IsOpen)
						m_SelectedPartitionStates.Remove(partition);
				}
			}
			finally
			{
				m_PartitionSection.Leave();
			}

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
			m_PartitionSection.Enter();
			try
			{
				foreach (var kvp in m_SelectedPartitionStates.ToArray())
					m_SubscribedPartitionManager.SetPartition<ConnectProCombineRoom>(kvp.Key, kvp.Value);

				m_SelectedPartitionStates.Clear();
			}
			finally
			{
				m_PartitionSection.Leave();
			}

			RefreshIfVisible();
		}

		private void ViewOnClearAllButtonPressed(object sender, EventArgs eventArgs)
		{
			m_PartitionSection.Execute(() => m_SelectedPartitionStates.Clear());
			RefreshIfVisible();
		}

		private void ViewOnWallButtonPressed(object sender, CellDirectionEventArgs args)
		{
			IcdConsole.PrintLine(eConsoleColor.Magenta, "Partition ({0}, {1}) {2} Pressed", args.Column, args.Row, args.Direction);
			
			var partition = m_SubscribedPartitionManager.GetPartition(args.Column, args.Row, args.Direction);
			if (partition == null)
				return;

			m_PartitionSection.Enter();
			try
			{
				if (m_SelectedPartitionStates.ContainsKey(partition))
					m_SelectedPartitionStates.Remove(partition);
				else
				{
					var controls = partition.GetPartitionControls()
					                        .Where(c => c.Mode.HasFlag(ePartitionFeedback.Set))
					                        .Select(info => Room.Core.GetControl(info.Control) as IPartitionDeviceControl)
					                        .Where(c => c != null).ToList();

					if (!controls.Any())
						return;

					m_SelectedPartitionStates.Add(partition, !controls.Any(c => c.IsOpen));
				}
			}
			finally
			{
				m_PartitionSection.Leave();
			}
			RefreshIfVisible();
		}

		#endregion
	}
}
