using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public class AbstractWtcPresenter<T> : AbstractUiPresenter<T>, IWtcPresenter<T> where T : class, IUiView
	{
		public AbstractWtcPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		private IWebConferenceDeviceControl m_ConferenceControl;
		public IWebConferenceDeviceControl ActiveConferenceControl
		{
			protected get { return m_ConferenceControl; }
			set
			{
				if (m_ConferenceControl == value)
					return;

				Unsubscribe(m_ConferenceControl);
				m_ConferenceControl = value;
				Subscribe(m_ConferenceControl);

				RefreshIfVisible();
			}
		}

		protected virtual void Subscribe(IWebConferenceDeviceControl control)
		{
			
		}

		protected virtual void Unsubscribe(IWebConferenceDeviceControl control)
		{
			
		}
	}
}