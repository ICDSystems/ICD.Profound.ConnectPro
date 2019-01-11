using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.TextControls;
using ICD.Connect.UI.Controls.Images;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views.WebConference.Contacts
{
	public partial class WtcReferencedSelectedContactView
	{
		private VtProSubpage m_Subpage;
		private VtProSimpleLabel m_NameLabel;
		private VtProImageObject m_AvatarImage;
		private VtProButton m_RemoveButton;

		protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
		{
			m_Subpage = new VtProSubpage(panel, parent, index);

			m_NameLabel = new VtProSimpleLabel(panel, m_Subpage)
			{
				IndirectTextJoin = 1
			};

			m_AvatarImage = new VtProImageObject(panel, m_Subpage)
			{
				DigitalVisibilityJoin = 2,
				SerialGraphicsJoin = 2
			};

			m_RemoveButton = new VtProButton(panel, m_Subpage)
			{
				DigitalPressJoin = 1
			};
		}

		protected override IEnumerable<IVtProControl> GetChildren()
		{
			yield return m_Subpage;
			yield return m_NameLabel;
			yield return m_AvatarImage;
			yield return m_RemoveButton;
		}
	}
}