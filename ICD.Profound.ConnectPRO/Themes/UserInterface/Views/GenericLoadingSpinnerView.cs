using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Views
{
	[ViewBinding(typeof(IGenericLoadingSpinnerView))]
	public sealed partial class GenericLoadingSpinnerView : AbstractUiView, IGenericLoadingSpinnerView
	{
		public GenericLoadingSpinnerView(ISigInputOutput panel, ConnectProTheme theme) : base(panel, theme)
		{
		}

		public void SetText(string text)
		{
			m_Label.SetLabelText(text);
		}
	}
}