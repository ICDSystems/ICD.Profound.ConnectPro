using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Connect.UI.Controls.Buttons;
using ICD.Connect.UI.Controls.Pages;
using ICD.Connect.UI.Controls.TextControls;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Schedule
{
    public sealed partial class ReferencedBookingView
    {
        private VtProSubpage m_Subpage;
        private VtProSimpleLabel m_TimeLabel;
        private VtProSimpleLabel m_SubjectLabel;
        private VtProButton m_Button;

        /// <summary>
        /// Instantiates the view controls.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        protected override void InstantiateControls(ISigInputOutput panel, IVtProParent parent, ushort index)
        {
            m_Subpage = new VtProSubpage(panel, parent, index);

            m_TimeLabel = new VtProSimpleLabel(panel, m_Subpage)
            {
                IndirectTextJoin = 1
            };

            m_SubjectLabel = new VtProSimpleLabel(panel, m_Subpage)
            {
                IndirectTextJoin = 2
            };

            m_Button = new VtProButton(panel, m_Subpage)
            {
                DigitalPressJoin = 1,
                DigitalEnableJoin = 2
            };
        }

        protected override IEnumerable<IVtProControl> GetChildren()
        {
            yield return m_Subpage;
            yield return m_TimeLabel;
            yield return m_SubjectLabel;
            yield return m_Button;
        }
    }
}