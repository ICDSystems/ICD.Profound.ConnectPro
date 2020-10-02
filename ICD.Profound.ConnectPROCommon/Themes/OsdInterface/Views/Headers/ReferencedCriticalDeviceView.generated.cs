using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Images;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Headers
{
	public partial class ReferencedCriticalDeviceView
	{
		private VtProSubpage m_Subpage;
		private VtProDynamicIconObject m_SubjectIcon;
		private VtProSimpleLabel m_SubjectLabel;

		/// <summary>
		/// Instantiates the view controls.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="parent"></param>
		/// <param name="index"></param>
		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_SubjectIcon = new VtProDynamicIconObject(panel, m_Subpage)
			{
				DynamicIconSerialJoin = 1
			};

			m_SubjectLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 2
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_SubjectIcon;
			yield return m_SubjectLabel;
		}
	}
}
