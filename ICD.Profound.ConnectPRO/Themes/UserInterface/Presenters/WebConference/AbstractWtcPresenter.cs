using ICD.Common.Properties;
using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public abstract class AbstractWtcPresenter<T> : AbstractUiPresenter<T>, IWtcPresenter<T>
		where T : class, IUiView
	{
		private IWebConferenceDeviceControl m_ConferenceControl;

		/// <summary>
		/// Gets/sets the active conference control for this presenter.
		/// </summary>
		[CanBeNull]
		public IWebConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_ConferenceControl; }
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

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		protected AbstractWtcPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		/// <summary>
		/// Subscribe to the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected virtual void Subscribe(IWebConferenceDeviceControl control)
		{
		}

		/// <summary>
		/// Unsubscribe from the conference control events.
		/// </summary>
		/// <param name="control"></param>
		protected virtual void Unsubscribe(IWebConferenceDeviceControl control)
		{
		}
	}
}
