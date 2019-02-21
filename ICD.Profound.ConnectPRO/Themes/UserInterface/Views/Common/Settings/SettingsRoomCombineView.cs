using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Panels;
using ICD.Connect.Partitioning.Partitions;
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
		public event EventHandler<PartitionDirectionEventArgs> OnWallButtonPressed;

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

		public void SetWallVisible(int column, int row, ePartitionDirection direction, bool visible)
		{
			GetWallButton(column, row, direction).Show(visible);
		}

		public void SetWallEnabled(int column, int row, ePartitionDirection direction, bool enabled)
		{
			GetWallButton(column, row, direction).Enable(enabled);
		}

		public void SetWallSelected(int column, int row, ePartitionDirection direction, bool selected)
		{
			GetWallButton(column, row, direction).SetSelected(selected);
		}

		#endregion

		#region Private Methods

		private VtProSimpleLabel GetLabel(int column, int row)
		{
			int index = column + row * COLUMNS;
			return m_CellLabels[index];
		}

		private VtProButton GetWallButton(int column, int row, ePartitionDirection direction)
		{
			throw new NotImplementedException();
		}

		private void CalculateColumnRowDirection(VtProButton wallButton, out int column, out int row,
		                                         out ePartitionDirection direction)
		{
			throw new NotImplementedException();
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

			foreach (VtProButton wall in m_WallButtons.Values)
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

			foreach (VtProButton wall in m_WallButtons.Values)
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
			VtProButton wall = sender as VtProButton;
			if (wall == null)
				return;

			int column;
			int row;
			ePartitionDirection direction;

			CalculateColumnRowDirection(wall, out column, out row, out direction);

			OnWallButtonPressed.Raise(this, new PartitionDirectionEventArgs(direction, column, row));
		}

		#endregion
	}
}
