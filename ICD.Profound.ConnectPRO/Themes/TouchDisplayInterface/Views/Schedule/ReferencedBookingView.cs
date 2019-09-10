using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Schedule;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views.Schedule
{
	[ViewBinding(typeof(IReferencedBookingView))]
	public sealed partial class ReferencedBookingView : AbstractTouchDisplayComponentView, IReferencedBookingView
	{
		public ReferencedBookingView(ISigInputOutput panel, ConnectProTheme theme, IVtProParent parent, ushort index)
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