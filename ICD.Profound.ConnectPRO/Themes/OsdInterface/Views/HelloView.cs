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
	}
}
