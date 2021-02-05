using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	[ViewBinding(typeof(IGenericLoadingSpinnerView))]
	public sealed partial class GenericLoadingSpinnerView : AbstractUiView, IGenericLoadingSpinnerView
	{
		public GenericLoadingSpinnerView(ISigInputOutput panel, IConnectProTheme theme)
			: base(panel, theme)
		{
		}

		public void SetText(string text)
		{
			m_Label.SetLabelText(text);
		}
	}
}