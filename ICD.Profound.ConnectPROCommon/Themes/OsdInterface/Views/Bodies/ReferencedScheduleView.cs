using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Bodies;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Bodies
{
	[ViewBinding(typeof(IReferencedScheduleView))]
	public sealed partial class ReferencedScheduleView : AbstractOsdView, IReferencedScheduleView
	{
		public ReferencedScheduleView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public ReferencedScheduleView(ISigInputOutput panel, IConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public void SetTimeLabel(string text)
		{
			m_TimeLabel.SetLabelText(text);
		}

		public void SetSubjectLabel(string text)
		{
			m_SubjectLabel.SetLabelText(text);
		}
	}
}