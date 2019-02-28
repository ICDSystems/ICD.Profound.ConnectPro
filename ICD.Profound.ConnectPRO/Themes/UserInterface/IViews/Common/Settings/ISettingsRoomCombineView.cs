using System;
using ICD.Connect.Partitioning.Cells;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings
{
	public interface ISettingsRoomCombineView : IUiView
	{
		event EventHandler OnClearAllButtonPressed;

		event EventHandler OnSaveButtonPressed;

		event EventHandler<CellDirectionEventArgs> OnWallButtonPressed; 

		void SetClearAllButtonEnabled(bool enabled);

		void SetSaveButtonEnabled(bool enabled);

		void SetCellVisible(int column, int row, bool visible);

		void SetCellLabel(int column, int row, string label);

		void SetWallEnabled(int column, int row, eCellDirection direction, bool enabled);

		void SetWallSelected(int column, int row, eCellDirection direction, bool selected);

		void SetWallMode(int column, int row, eCellDirection direction, eWallButtonMode mode);
	}

	public enum eWallButtonMode : ushort
	{
		NoWall = 0,
		PermanentWall = 1,
		ClosedPartition = 2,
		OpenPartition = 3,
		UnsavedClosedPartition = 4,
		UnsavedOpenPartition = 5
	}
}
