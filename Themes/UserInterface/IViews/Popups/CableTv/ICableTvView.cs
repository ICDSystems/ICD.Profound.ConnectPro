using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.CableTv
{
	public interface ICableTvView : IPopupView
	{
		event EventHandler OnGuideButtonPressed;
		event EventHandler OnExitButtonPressed;
		event EventHandler OnPowerButtonPressed;

		event EventHandler<CharEventArgs> OnNumberButtonPressed;
		event EventHandler OnClearButtonPressed;
		event EventHandler OnEnterButtonPressed;

		event EventHandler OnUpButtonPressed;
		event EventHandler OnDownButtonPressed;
		event EventHandler OnLeftButtonPressed;
		event EventHandler OnRightButtonPressed;
		event EventHandler OnSelectButtonPressed;

		event EventHandler OnChannelUpButtonPressed;
		event EventHandler OnChannelDownButtonPressed;
		event EventHandler OnPageUpButtonPressed;
		event EventHandler OnPageDownButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IReferencedCableTvView> GetChildComponentViews(IViewFactory factory, ushort count);

		void ShowSwipeIcons(bool show);
	}
}
