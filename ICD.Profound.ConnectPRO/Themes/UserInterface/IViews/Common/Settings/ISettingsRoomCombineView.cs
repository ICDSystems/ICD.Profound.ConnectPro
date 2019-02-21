using System;
using ICD.Connect.Partitioning.Partitions;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings
{
	public interface ISettingsRoomCombineView : IUiView
	{
		event EventHandler OnClearAllButtonPressed;

		event EventHandler OnSaveButtonPressed;

		event EventHandler<PartitionDirectionEventArgs> OnWallButtonPressed; 

		void SetClearAllButtonEnabled(bool enabled);

		void SetSaveButtonEnabled(bool enabled);

		void SetCellVisible(int column, int row, bool visible);

		void SetCellLabel(int column, int row, string label);

		void SetWallVisible(int column, int row, ePartitionDirection direction, bool visible);

		void SetWallEnabled(int column, int row, ePartitionDirection direction, bool enabled);

		void SetWallSelected(int column, int row, ePartitionDirection direction, bool selected);
	}
}
