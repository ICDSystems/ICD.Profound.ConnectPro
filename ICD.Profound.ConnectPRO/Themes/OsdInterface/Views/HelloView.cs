using System.Collections.Generic;
using ICD.Connect.Panels;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPRO.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.OsdInterface.Views
{
	public sealed partial class HelloView : AbstractOsdView, IHelloView
	{
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
			m_Label.Enable(mainPageView);
		}
	}
}
