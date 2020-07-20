using System;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.FooterNotifications;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.FooterNotifications
{
	public interface IOsdHelloFooterNotificationPresenter : IOsdPresenter<IOsdHelloFooterNotificationView>
	{
		event EventHandler<BoolEventArgs> OnMainPageViewChanged;

		/// <summary>
		/// Gets whether the presenter is in the main area (true) or the notification area (false)
		/// </summary>
		bool MainPageView { get; }

		/// <summary>
		/// Adds the message to the top of the stack and refreshes the view.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		void PushMessage(string key, string message);

		/// <summary>
		/// Removes the messages from the stack with the given key and refreshes the view.
		/// </summary>
		/// <param name="key"></param>
		void ClearMessages(string key);
	}
}
