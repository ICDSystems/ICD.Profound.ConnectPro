using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views
{
	[ViewBinding(typeof(IHelloView))]
	public sealed partial class HelloView : AbstractTouchDisplayView, IHelloView
	{
		public HelloView(ISigInputOutput panel, TouchCueTheme theme)
			: base(panel, theme)
		{
		}

		public void SetLabelText(string text)
		{
			m_Label.SetLabelText(text);
		}

		/// <summary>
		/// Sets whether the hello text is in the Main View (true, no scheduler and out of meeting)
		/// or in the notification section (false, scheduler or in meeting
		/// </summary>
		/// <param name="mainPageView"></param>
		public void SetMainPageView(bool mainPageView)
		{
			// TODO: hack, replace when we make OSD specific controls
			m_Label.Enable(mainPageView);
			m_Label.Show(!mainPageView);
		}
	}
}