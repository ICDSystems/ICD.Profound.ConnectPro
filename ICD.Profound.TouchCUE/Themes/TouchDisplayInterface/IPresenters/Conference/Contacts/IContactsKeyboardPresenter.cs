using System;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.Conference.Contacts;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.Conference.Contacts
{
	public interface IContactsKeyboardPresenter : ITouchDisplayPresenter<IContactsKeyboardView>
	{
		/// <summary>
		/// Shows the view using the given callback for the enter button.
		/// </summary>
		/// <param name="prompt"></param>
		/// <param name="text"></param>
		/// <param name="enterCallback"></param>
		/// <param name="closeCallback"></param>
		/// <param name="changeCallback"></param>
		void ShowView(string prompt, string text, Action<string> closeCallback, Action<string> changeCallback);
	}
}
