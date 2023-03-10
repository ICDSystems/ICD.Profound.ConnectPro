using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Mvp.Views;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.VideoConference.Dtmf
{
	public interface IVtcDtmfView : IUiView
	{
		/// <summary>
		/// Raised when the user presses a dialtone button.
		/// </summary>
		event EventHandler<CharEventArgs> OnToneButtonPressed;

		/// <summary>
		/// Returns child views for list items.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		IEnumerable<IVtcReferencedDtmfView> GetChildComponentViews(IViewFactory factory, ushort count);
	}
}
