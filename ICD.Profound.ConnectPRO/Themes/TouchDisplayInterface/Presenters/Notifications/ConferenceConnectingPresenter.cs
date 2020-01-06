using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Notifications;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Notifications;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Notifications
{
	[PresenterBinding(typeof(IConferenceConnectingPresenter))]
	public sealed class ConferenceConnectingPresenter : AbstractTouchDisplayPresenter<IConferenceConnectingView>, IConferenceConnectingPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private string m_NotificationText;

		public ConferenceConnectingPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		protected override void Refresh(IConferenceConnectingView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				view.SetNotificationText(m_NotificationText);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		public void Show(string notificationText)
		{
			m_NotificationText = notificationText;
			ShowView(true);
		}

		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_NotificationText = string.Empty;
		}
	}
}