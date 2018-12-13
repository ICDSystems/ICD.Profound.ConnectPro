using System;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews
{
	public interface IGenericAlertView : IUiView
	{
		event EventHandler OnDismissButtonPressed;

		void SetAlertText(string text);

		void SetDismissButtonEnabled(bool enable);
	}
}