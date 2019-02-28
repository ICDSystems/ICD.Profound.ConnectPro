using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Partitioning.Cells;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.TextControls;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	[ViewBinding(typeof(ISettingsRoomCombineView))]
	public sealed partial class SettingsRoomCombineView : AbstractUiView, ISettingsRoomCombineView
	{
		public event EventHandler OnClearAllButtonPressed;
		public event EventHandler OnSaveButtonPressed;
		public event EventHandler<CellDirectionEventArgs> OnWallButtonPressed;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="theme"></param>
		public SettingsRoomCombineView(ISigInputOutput panel, ConnectProTheme theme)
			: base(panel, theme)
		{
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnClearAllButtonPressed = null;
			OnSaveButtonPressed = null;
			OnWallButtonPressed = null;

			base.Dispose();
		}

		#region Methods

		public void SetClearAllButtonEnabled(bool enabled)
		{
			m_ClearAllButton.Enable(enabled);
		}

		public void SetSaveButtonEnabled(bool enabled)
		{
			m_SaveButton.Enable(enabled);
		}

		public void SetCellVisible(int column, int row, bool visible)
		{
			GetLabel(column, row).Show(visible);
		}

		public void SetCellLabel(int column, int row, string label)
		{
			GetLabel(column, row).SetLabelText(label);
		}

		public void SetWallEnabled(int column, int row, eCellDirection direction, bool enabled)
		{
			GetWallButton(column, row, direction).Enable(enabled);
		}

		public void SetWallSelected(int column, int row, eCellDirection direction, bool selected)
		{
			GetWallButton(column, row, direction).SetSelected(selected);
		}

		public void SetWallMode(int column, int row, eCellDirection direction, eWallButtonMode mode)
		{
			GetWallButton(column, row, direction).SetMode(mode.ToUShort());
		}

		#endregion

		#region Private Methods

		private VtProSimpleLabel GetLabel(int column, int row)
		{
			int index = column + row * COLUMNS;
			return m_CellLabels[index];
		}

		private VtProMultiModeButton GetWallButton(int column, int row, eCellDirection direction)
		{
			// warning - wacky math
			// switch things to be referencing the top and left walls (makes it a bit easier)
			if (direction == eCellDirection.Right)
			{
				column++;
				direction = eCellDirection.Left;
			}
			else if (direction == eCellDirection.Bottom)
			{
				row++;
				direction = eCellDirection.Top;
			}

			int wallsPerRow = 2 * COLUMNS + 1; // COLUMNS number of top walls, COLUMNS number of left walls, 1 right wall

			int topWall = row * wallsPerRow + column; // get index of top wall
			int index = topWall + (direction == eCellDirection.Left ? COLUMNS : 0); // left wall is offset from top wall by COLUMNS index

			return m_WallButtons.GetValue(index);
		}

		private void CalculateColumnRowDirection(VtProMultiModeButton wallButton, out int column, out int row,
		                                         out eCellDirection direction)
		{
			// todo - you could return the cell/direction on either side
			int index = m_WallButtons.GetKey(wallButton);
			int wallsPerRow = 2 * COLUMNS + 1;
			row = index / wallsPerRow;
			if (row >= ROWS) // handle the bottom walls of the last row
			{
				row--;
				column = index % wallsPerRow;
				direction = eCellDirection.Bottom;
				return;
			}

			int rowIndex = index % wallsPerRow; // get index within row

			int dir = rowIndex / COLUMNS; // 0 is top, 1 is left, 2 is right
			if (dir == 2) // handle right walls on last column
			{
				column = COLUMNS - 1;
				direction = eCellDirection.Right;
				return;
			}
			column = rowIndex % COLUMNS;
			direction = dir == 0 ? eCellDirection.Top : eCellDirection.Left;
		}

		#endregion

		#region Control Callbacks

		/// <summary>
		/// Subscribes to the view controls.
		/// </summary>
		protected override void SubscribeControls()
		{
			base.SubscribeControls();

			m_SaveButton.OnPressed += SaveButtonOnPressed;
			m_ClearAllButton.OnPressed += ClearAllButtonOnPressed;

			foreach (VtProMultiModeButton wall in m_WallButtons.Values)
				wall.OnPressed += WallOnPressed;
		}

		/// <summary>
		/// Unsubscribes from the view controls.
		/// </summary>
		protected override void UnsubscribeControls()
		{
			base.UnsubscribeControls();

			m_SaveButton.OnPressed -= SaveButtonOnPressed;
			m_ClearAllButton.OnPressed -= ClearAllButtonOnPressed;

			foreach (VtProMultiModeButton wall in m_WallButtons.Values)
				wall.OnPressed -= WallOnPressed;
		}

		private void ClearAllButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnClearAllButtonPressed.Raise(this);
		}

		private void SaveButtonOnPressed(object sender, EventArgs eventArgs)
		{
			OnSaveButtonPressed.Raise(this);
		}

		private void WallOnPressed(object sender, EventArgs eventArgs)
		{
			VtProMultiModeButton wall = sender as VtProMultiModeButton;
			if (wall == null)
				return;

			int column;
			int row;
			eCellDirection direction;

			CalculateColumnRowDirection(wall, out column, out row, out direction);

			OnWallButtonPressed.Raise(this, new CellDirectionEventArgs(direction, column, row));
		}

		#endregion
	}
}
