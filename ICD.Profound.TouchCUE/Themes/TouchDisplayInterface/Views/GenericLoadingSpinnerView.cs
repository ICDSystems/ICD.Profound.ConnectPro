using ICD.Connect.Panels;
using ICD.Connect.UI.Attributes;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.Views
{
	[ViewBinding(typeof(IGenericLoadingSpinnerView))]
	public sealed partial class GenericLoadingSpinnerView : AbstractTouchDisplayView, IGenericLoadingSpinnerView
	{
		public GenericLoadingSpinnerView(ISigInputOutput panel, TouchCueTheme theme) : base(panel, theme)
		{
		}

		public void SetText(string text)
		{
			m_Label.SetLabelText(text);
		}
	}
}