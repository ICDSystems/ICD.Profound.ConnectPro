using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;
using ICD.Common.Utils.Collections;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common.FloatingActions
{
	public sealed partial class FloatingActionListView
	{
		private const int NUMBER_OF_ITEMS = 7;

		private const int BUTTON_PRESS_JOIN_START = 1301;

		private const int BUTTON_VISIBILITY_JOIN_START = 1351;

		private const int DYNAMIC_ICON_SERIAL_JOIN_START = 1351;

		private const int LABEL_INDIRECT_TEXT_JOIN_START = 1301;

		private VtProSubpage m_Subpage;

		private BiDictionary<int, VtProButton> m_Buttons;

		private Dictionary<int, VtProSimpleLabel> m_Labels;

		private Dictionary<int, VtProDynamicIconObject> m_Icons;

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
				DigitalVisibilityJoin = 975
			};

			m_Buttons = new BiDictionary<int, VtProButton>();
			for (int i = 0;i < NUMBER_OF_ITEMS; i++)
			{
				var button = new VtProButton(panel, parent)
				{
					DigitalPressJoin = (ushort)(BUTTON_PRESS_JOIN_START + i),
					DigitalVisibilityJoin = (ushort)(BUTTON_VISIBILITY_JOIN_START + i)
				};
				m_Buttons.Add(i,button);
			}

			m_Icons = new Dictionary<int, VtProDynamicIconObject>(NUMBER_OF_ITEMS);
			for (int i = 0; i < NUMBER_OF_ITEMS; i++)
			{
				var icon = new VtProDynamicIconObject(panel, parent)
				{
					DynamicIconSerialJoin = (ushort)(DYNAMIC_ICON_SERIAL_JOIN_START + i)
				};
				m_Icons.Add(i,icon);
			}

			m_Labels = new Dictionary<int, VtProSimpleLabel>(NUMBER_OF_ITEMS);
			for (int i = 0; i < NUMBER_OF_ITEMS; i++)
			{
				var label = new VtProSimpleLabel(panel, parent)
				{
					IndirectTextJoin =  (ushort)(LABEL_INDIRECT_TEXT_JOIN_START + i)
				};
				m_Labels.Add(i,label);
			}
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			foreach (VtProButton button in m_Buttons.Values)
				yield return button;
			foreach (VtProDynamicIconObject icon in m_Icons.Values)
				yield return icon;
			foreach (VtProSimpleLabel label in m_Labels.Values)
				yield return label;
		}
	}
}
