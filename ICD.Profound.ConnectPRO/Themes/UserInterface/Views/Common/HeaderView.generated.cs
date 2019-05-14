using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Common
{
	public sealed partial class HeaderView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_RoomNameLabel;
		private VtProSimpleLabel m_TimeLabel;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_RoomNameLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 10
			};

			m_TimeLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 9
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_RoomNameLabel;
			yield return m_TimeLabel;
		}
	}
}
