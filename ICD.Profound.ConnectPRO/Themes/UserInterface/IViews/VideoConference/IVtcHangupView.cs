using System;
using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference
{
	public interface IVtcHangupView : IView
	{
		/// <summary>
		/// Raised when the user presses the hangup all button.
		/// </summary>
		event EventHandler OnHangupAllButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IVtcReferencedHangupView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
