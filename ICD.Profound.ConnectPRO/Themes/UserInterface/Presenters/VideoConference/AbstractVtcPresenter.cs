using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public abstract class AbstractVtcPresenter<T> : AbstractUiPresenter<T>, IVtcPresenter where T : class, IUiView
	{
		private IConferenceDeviceControl m_ActiveConferenceControl;
		public IConferenceDeviceControl ActiveConferenceControl
		{
			protected get { return m_ActiveConferenceControl; }
			set
			{
				if (m_ActiveConferenceControl == value)
					return;

				if (m_ActiveConferenceControl != null)
					Unsubscribe(m_ActiveConferenceControl);

				m_ActiveConferenceControl = value;

				if (m_ActiveConferenceControl != null)
					Subscribe(m_ActiveConferenceControl);

				RefreshIfVisible();
			}
		}

		protected AbstractVtcPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		protected virtual void Subscribe(IConferenceDeviceControl control)
		{
		}

		protected virtual void Unsubscribe(IConferenceDeviceControl control)
		{
		}
	}
}