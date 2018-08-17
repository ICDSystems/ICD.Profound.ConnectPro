﻿using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.ActiveCalls
{
	public interface IVtcActiveCallsView : IView
	{
		/// <summary>
		/// Raised when the user presses the hangup all button.
		/// </summary>
		event EventHandler OnHangupAllButtonPressed;

		/// <summary>
		/// Raised when the user presses the close button.
		/// </summary>
		event EventHandler OnCloseButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IVtcReferencedActiveCallsView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
