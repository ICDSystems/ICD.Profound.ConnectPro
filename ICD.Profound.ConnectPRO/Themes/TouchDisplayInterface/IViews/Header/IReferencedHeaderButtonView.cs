using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Header
{
	public interface IReferencedHeaderButtonView : ITouchDisplayView
	{
		event EventHandler OnPressed;

		void SetLabelText(string text);

		void SetIcon(string icon);

		void SetButtonMode(eHeaderButtonMode mode);

		void SetButtonEnabled(bool enabled);
	}
	
	public enum eHeaderButtonMode : ushort
	{
		Red = 0,
		Orange = 1,
		Green = 2,
		Blue = 3,
		Close = 4
	}
}