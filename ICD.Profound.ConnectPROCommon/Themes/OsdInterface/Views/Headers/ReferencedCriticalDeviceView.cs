using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Controls;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Views.Headers
{
	[ViewBinding(typeof(IReferencedCriticalDeviceView))]
	public partial class  ReferencedCriticalDeviceView : AbstractOsdView, IReferencedCriticalDeviceView
	{
		public ReferencedCriticalDeviceView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public ReferencedCriticalDeviceView(ISigInputOutput panel, IConnectProTheme theme, IVtProParent parent, ushort index)
			: base(panel, theme, parent, index)
		{
		}

		public void SetSubjectIcon(string icon)
		{
			m_SubjectIcon.SetIcon(icon);
		}

		public void SetSubjectLabel(string text)
		{
			m_SubjectLabel.SetLabelText(text);
		}
	}
}
