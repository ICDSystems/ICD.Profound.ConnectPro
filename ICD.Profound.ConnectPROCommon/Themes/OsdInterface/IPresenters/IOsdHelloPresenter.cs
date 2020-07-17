using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters
{
	public interface IOsdHelloPresenter : IOsdPresenter<IOsdHelloView>
	{
		event EventHandler<BoolEventArgs> OnMainPageViewChanged;

		/// <summary>
		/// Gets whether the presenter is in the main area (true) or the notification area (false)
		/// </summary>
		bool MainPageView { get; }

		/// <summary>
		/// Adds the message to the top of the stack and refreshes the view.
		/// </summary>
		/// <param name="message"></param>
		void PushMessage(string message);

		/// <summary>
		/// Removes the message from the stack and refreshes the view.
		/// </summary>
		/// <param name="message"></param>
		void PopMessage(string message);
	}
}
