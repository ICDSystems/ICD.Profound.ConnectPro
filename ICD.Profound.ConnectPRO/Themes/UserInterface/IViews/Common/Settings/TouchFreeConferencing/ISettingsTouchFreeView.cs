using System;
using System.Collections.Generic;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing
{
	public interface ISettingsTouchFreeView : IUiView
	{
		event EventHandler OnCountDownTimerIncrementButtonPressed;
		event EventHandler OnCountDownTimerDecrementButtonPressed;
		event EventHandler OnIncrementDecrementButtonReleased;
		event EventHandler OnEnableZeroTouchTogglePressed;

		void SetTouchFreeToggleSelected(bool selected);

		void SetCountDownSeconds(int seconds);

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedSettingsTouchFreeView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}