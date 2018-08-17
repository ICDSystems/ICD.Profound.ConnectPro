using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.Popups.CableTv
{
	public sealed partial class ReferencedCableTvView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicIconObject m_Icon;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_Icon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DigitalPressJoin = 1,
				DynamicIconSerialJoin = 1,
				IndirectGraphicsPathSerialJoin = 1
			};
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_Icon;
		}
	}
}
