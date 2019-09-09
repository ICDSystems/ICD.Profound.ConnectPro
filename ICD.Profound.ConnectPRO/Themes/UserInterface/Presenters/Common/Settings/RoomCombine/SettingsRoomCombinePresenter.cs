using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Partitioning.Cells;
using ICD.Connect.Partitioning.Controls;
using ICD.Connect.Partitioning.PartitionManagers;
using ICD.Connect.Partitioning.Partitions;
using ICD.Connect.Partitioning.Rooms;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Utils;
using ICD.Profound.ConnectPRO.Rooms;
using ICD.Profound.ConnectPRO.Rooms.Combine;
using ICD.Profound.ConnectPRO.SettingsTree.RoomCombine;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.RoomCombine;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.RoomCombine;
using ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings.RoomCombine;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.RoomCombine
{
	[PresenterBinding(typeof(ISettingsRoomCombinePresenter))]
	public sealed class SettingsRoomCombinePresenter :
		AbstractSettingsNodeBasePresenter<ISettingsRoomCombineView, GridSettingsLeaf>, ISettingsRoomCombinePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeCriticalSection m_PartitionSection;

		/// <summary>
		/// Tracks the current selection state for the walls. True is open, false is close.
		/// </summary>
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

						string roomLabel = room == null ? null : room.Name;
						if (room != null && Room != null && (room == Room || Room.ContainsRoom(room)))
							roomLabel = HtmlUtils.FormatColoredText(roomLabel, Colors.COLOR_DARK_BLUE);

						view.SetCellVisible(column, row, room != null);
						view.SetCellLabel(column, row, roomLabel);
						view.SetCellSelected(column, row, room != null);
						view.SetCellEnabled(column, row, room != null);

						bool topEnabled = GetWallEnabled(column, row, eCellDirection.Top);
						bool topSelected = GetWallSelected(column, row, eCellDirection.Top);
						eWallButtonMode topMode = GetWallMode(column, row, eCellDirection.Top);

						view.SetWallEnabled(column, row, eCellDirection.Top, topEnabled);
						view.SetWallSelected(column, row, eCellDirection.Top, topSelected);
						view.SetWallVisible(column, row, eCellDirection.Top, topMode != eWallButtonMode.NoWall);
						view.SetWallMode(column, row, eCellDirection.Top, topMode);

						bool leftEnabled = GetWallEnabled(column, row, eCellDirection.Left);
						bool leftSelected = GetWallSelected(column, row, eCellDirection.Left);
						eWallButtonMode leftMode = GetWallMode(column, row, eCellDirection.Left);

						view.SetWallEnabled(column, row, eCellDirection.Left, leftEnabled);
						view.SetWallSelected(column, row, eCellDirection.Left, leftSelected);
						view.SetWallVisible(column, row, eCellDirection.Left, leftMode != eWallButtonMode.NoWall);
						view.SetWallMode(column, row, eCellDirection.Left, leftMode);

						if (column == SettingsRoomCombineView.COLUMNS - 1)
						{
							bool rightEnabled = GetWallEnabled(column, row, eCellDirection.Right);
							bool rightSelected = GetWallSelected(column, row, eCellDirection.Right);
							eWallButtonMode rightMode = GetWallMode(column, row, eCellDirection.Right);

							view.SetWallEnabled(column, row, eCellDirection.Right, rightEnabled);
							view.SetWallSelected(column, row, eCellDirection.Right, rightSelected);
							view.SetWallVisible(column, row, eCellDirection.Right, rightMode != eWallButtonMode.NoWall);
							view.SetWallMode(column, row, eCellDirection.Right, rightMode);
						}

						if (row == SettingsRoomCombineView.ROWS - 1)
						{
							bool bottomEnabled = GetWallEnabled(column, row, eCellDirection.Bottom);
							bool bottomSelected = GetWallSelected(column, row, eCellDirection.Bottom);
							eWallButtonMode bottomMode = GetWallMode(column, row, eCellDirection.Bottom);

							view.SetWallEnabled(column, row, eCellDirection.Bottom, bottomEnabled);
							view.SetWallSelected(column, row, eCellDirection.Bottom, bottomSelected);
							view.SetWallVisible(column, row, eCellDirection.Bottom, bottomMode != eWallButtonMode.NoWall);
							view.SetWallMode(column, row, eCellDirection.Bottom, bottomMode);
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

		private eWallButtonMode GetWallMode(int column, int row, eCellDirection direction)
		{
			if (m_SubscribedPartitionManager == null)
				return eWallButtonMode.NoWall;

			ICell cell = m_SubscribedPartitionManager.Cells.GetCell(column, row);
			ICell neighboringCell = m_SubscribedPartitionManager.Cells.GetNeighboringCell(column, row, direction);
			IRoom room = cell == null ? null : cell.Room;
			IRoom neighboringRoom = neighboringCell == null ? null : neighboringCell.Room;

			// If both aren't rooms, then no wall
			if (room == null && neighboringRoom == null)
				return eWallButtonMode.NoWall;

			// If same room, don't show wall
			if (room == neighboringRoom)
				return eWallButtonMode.NoWall;

			// If room and no room, permanent wall
			if (room == null || neighboringRoom == null)
				return eWallButtonMode.PermanentWall;

			// If there's no partition, permanent wall
			IPartition partition = m_SubscribedPartitionManager.GetPartition(column, row, direction);
			if (partition == null)
				return eWallButtonMode.PermanentWall;

			// If user has manually selected a partition state, show that
			m_PartitionSection.Enter();
			try
			{
				if (m_SelectedPartitionStates.ContainsKey(partition))
					return m_SelectedPartitionStates[partition]
						       ? eWallButtonMode.UnsavedOpenPartition
						       : eWallButtonMode.UnsavedClosedPartition;
			}
			finally
			{
				m_PartitionSection.Leave();
			}

			// Does the partition currently combine multiple rooms?
			return m_SubscribedPartitionManager.CombinesRoom(partition)
				       ? eWallButtonMode.OpenPartition
				       : eWallButtonMode.ClosedPartition;
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

		protected override void ViewOnPreVisibilityChanged(object sender, BoolEventArgs args)
		{
			// Clear the selection when the visibility changes
			m_RefreshSection.Execute(() => m_SelectedPartitionStates.Clear());

			base.ViewOnPreVisibilityChanged(sender, args);
		}

		private void ViewOnSaveButtonPressed(object sender, EventArgs eventArgs)
		{
			m_PartitionSection.Enter();


			try
			{
				IcdHashSet<IPartition> open = m_SelectedPartitionStates
					.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToIcdHashSet();
				IcdHashSet<IPartition> closed = m_SelectedPartitionStates
					.Where(kvp => !kvp.Value).Select(kvp => kvp.Key).ToIcdHashSet();

				Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>().ShowView("Combining Rooms");

				m_SubscribedPartitionManager.CombineRooms(open, closed, () => new ConnectProCombineRoom());
				m_SelectedPartitionStates.Clear();
			}
			catch (Exception e)
			{
				Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>()
				          .TimeOut("Failed to complete operation - " + e.Message);
			}
			finally
			{
				m_PartitionSection.Leave();

				Navigation.LazyLoadPresenter<IGenericLoadingSpinnerPresenter>().ShowView(false);
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
			if (Room == null)
				return;

			IPartition partition = m_SubscribedPartitionManager.GetPartition(args.Column, args.Row, args.Direction);
			if (partition == null)
				return;

			m_PartitionSection.Enter();

			try
			{
				// Don't let the user open/close partitions outside of the current space
				IcdHashSet<IPartition> contiguousPartitions = GetContiguousPartitions().ToIcdHashSet();
				if (!contiguousPartitions.Contains(partition))
					return;

				if (m_SelectedPartitionStates.ContainsKey(partition))
				{
					// Clear the selection and update siblings to match the state
					foreach (IPartition sibling in
						m_SubscribedPartitionManager.Partitions.GetSiblingPartitions(partition))
						m_SelectedPartitionStates.Remove(sibling);
				}
				else
				{
					// We want to open the closed partition OR close the open partition
					bool open = !m_SubscribedPartitionManager.CombinesRoom(partition);

					// Update siblings to match the state
					foreach (IPartition sibling in
						m_SubscribedPartitionManager.Partitions.GetSiblingPartitions(partition))
						m_SelectedPartitionStates[sibling] = open;
				}

				// Select orphaned partitions for closing
				IcdHashSet<IPartition> newContiguousPartitions = GetContiguousPartitions().ToIcdHashSet();
				IEnumerable<IPartition> closeSelection =
					contiguousPartitions.Where(p => !newContiguousPartitions.Contains(p));

				foreach (IPartition close in closeSelection)
				{
					if (m_SubscribedPartitionManager.CombinesRoom(close))
						m_SelectedPartitionStates[close] = false;
					else
						m_SelectedPartitionStates.Remove(close);
				}
			}
			finally
			{
				m_PartitionSection.Leave();
			}

			RefreshIfVisible();
		}

		/// <summary>
		/// Get the current contiguous space based on open partitions and the selected open partitions.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IPartition> GetContiguousPartitions()
		{
			if (Room == null)
				return Enumerable.Empty<IPartition>();

			IRoom start = Room.GetRoomsRecursive().FirstOrDefault(r => r.Originators.Contains(ViewFactory.Panel.Id));
			if (start == null)
				return Enumerable.Empty<IPartition>();

			return RecursionUtils.GetClique(start, GetAdjacentRooms)
			                     .SelectMany(r => m_SubscribedPartitionManager.Partitions.GetRoomAdjacentPartitions(r))
			                     .Distinct();
		}

		/// <summary>
		/// Returns all of the rooms that are adjacent to the given room where the dividing partition is
		/// open or selected to open.
		/// </summary>
		/// <param name="room"></param>
		/// <returns></returns>
		private IEnumerable<IRoom> GetAdjacentRooms(IRoom room)
		{
			if (room == null)
				throw new ArgumentNullException("room");

			IcdHashSet<IRoom> adjacent =

				m_SubscribedPartitionManager
					.Partitions
					.GetRoomAdjacentPartitions(room)
					.Where(p =>
					       {
						       bool selection;
						       if (m_SelectedPartitionStates.TryGetValue(p, out selection))
							       return selection;

						       return m_SubscribedPartitionManager.CombinesRoom(p);
					       })
					.SelectMany(p => p.GetRooms().Select(id => room.Core.Originators.GetChild<IRoom>(id)))
					.Distinct()
					.Where(r => r != room)
					.ToIcdHashSet();

			return adjacent;
		}

		#endregion
	}
}
