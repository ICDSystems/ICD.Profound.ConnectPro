using System;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews
{
	public interface IGenericAlertView : ITouchDisplayView
	{
		event EventHandler OnDismissButtonPressed;

		void SetAlertText(string text);

		void SetDismissButtonEnabled(bool enable);
	}
}