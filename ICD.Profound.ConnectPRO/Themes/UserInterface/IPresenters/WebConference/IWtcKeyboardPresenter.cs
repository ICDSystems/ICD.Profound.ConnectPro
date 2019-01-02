﻿using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference
{
	public interface IWtcKeyboardPresenter : IUiPresenter<IWtcKeyboardView>
	{
		/// <summary>
		/// Shows the view using the given callback for the Enter button.
		/// </summary>
		/// <param name="enterButtonCallback"></param>
		void ShowView(Action<string> enterButtonCallback);
	}
}
