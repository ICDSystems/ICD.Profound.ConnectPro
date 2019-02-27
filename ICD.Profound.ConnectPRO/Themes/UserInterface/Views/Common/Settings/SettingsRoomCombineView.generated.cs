using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.Settings
{
	public sealed partial class SettingsRoomCombineView
	{
		public const int ROWS = 4;
		public const int COLUMNS = 4;

		private VtProSubpage m_Subpage;

		private VtProButton m_ClearAllButton;
		private VtProButton m_SaveButton;

		// From top-left to bottom-right
		private VtProSimpleLabel[] m_CellLabels;
		private BiDictionary<int, VtProMultiModeButton> m_WallButtons;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index)
			{
				DigitalVisibilityJoin = 145
			};

			m_ClearAllButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 990,
				DigitalEnableJoin = 991
			};

			m_SaveButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 992,
				DigitalEnableJoin = 993
			};

			m_CellLabels = Enumerable.Range(0, COLUMNS * ROWS)
			                         .Select(i => new VtProSimpleLabel(panel, parent)
			                         {
				                         DigitalVisibilityJoin = (ushort)(1000 + i),
										 IndirectTextJoin = (ushort)(1000 + i)
			                         }).ToArray();

			ushort currentJoin = 1016;
			ushort currentAnalogJoin = 1000;
			Dictionary<int, VtProMultiModeButton> wallButtons =
				Enumerable.Range(0, (COLUMNS * (ROWS + 1)) + (ROWS * (COLUMNS + 1)))
				          .ToDictionary(i => i, i => new VtProMultiModeButton(panel, m_Subpage)
				          {
					          DigitalPressJoin = currentJoin++,
					          DigitalEnableJoin = currentJoin++,
					          DigitalVisibilityJoin = currentJoin++,
							  AnalogModeJoin = currentAnalogJoin++
				          });

			m_WallButtons = new BiDictionary<int, VtProMultiModeButton>(wallButtons);
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_ClearAllButton;
			yield return m_SaveButton;

			foreach (VtProSimpleLabel label in m_CellLabels)
				yield return label;

			foreach (VtProMultiModeButton button in m_WallButtons.Values)
				yield return button;
		}
	}
}
