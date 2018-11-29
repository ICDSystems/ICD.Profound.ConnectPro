using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.VideoConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.VideoConference
{
	public abstract class AbstractVtcPresenter<T> : AbstractUiPresenter<T>, IVtcPresenter where T : class, IUiView
	{
		private ITraditionalConferenceDeviceControl m_ActiveConferenceControl;
		public ITraditionalConferenceDeviceControl ActiveConferenceControl
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

		protected AbstractVtcPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		protected virtual void Subscribe(ITraditionalConferenceDeviceControl control)
		{
		}

		protected virtual void Unsubscribe(ITraditionalConferenceDeviceControl control)
		{
		}
	}
}