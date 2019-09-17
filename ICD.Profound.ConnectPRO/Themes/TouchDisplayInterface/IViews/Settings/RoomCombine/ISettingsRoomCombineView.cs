using System;
using ICD.Connect.Partitioning.Cells;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Settings.RoomCombine
{
	public interface ISettingsRoomCombineView : ITouchDisplayView
	{
		event EventHandler OnClearAllButtonPressed;

		event EventHandler OnSaveButtonPressed;

		event EventHandler<CellDirectionEventArgs> OnWallButtonPressed; 

		void SetClearAllButtonEnabled(bool enabled);

		void SetSaveButtonEnabled(bool enabled);

		void SetCellVisible(int column, int row, bool visible);

		void SetCellLabel(int column, int row, string label);

		void SetCellSelected(int column, int row, bool selected);

		void SetCellEnabled(int column, int row, bool enabled);

		void SetWallEnabled(int column, int row, eCellDirection direction, bool enabled);

		void SetWallSelected(int column, int row, eCellDirection direction, bool selected);

		void SetWallVisible(int column, int row, eCellDirection direction, bool visible);

		void SetWallMode(int column, int row, eCellDirection direction, eWallButtonMode mode);
	}

	public enum eWallButtonMode : ushort
	{
		NoWall = 0,
		PermanentWall = 1,
		UnsavedClosedPartition = 2,
		ClosedPartition = 3,
		UnsavedOpenPartition = 4,
		OpenPartition = 5,
	}
}
