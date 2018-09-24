using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews.Welcome;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views.Welcome
{
	public sealed partial class ReferencedScheduleView : AbstractOsdView, IReferencedScheduleView
	{
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