using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Views
{
	[ViewBinding(typeof(IGenericLoadingSpinnerView))]
	public sealed partial class GenericLoadingSpinnerView : AbstractTouchDisplayView, IGenericLoadingSpinnerView
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