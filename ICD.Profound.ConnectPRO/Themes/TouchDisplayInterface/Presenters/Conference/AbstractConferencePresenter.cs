using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Connect.UI.Mvp.Views;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference
{
	public abstract class AbstractConferencePresenter<T> : AbstractTouchDisplayPresenter<T> where T : class, ITouchDisplayView
	{
		private IConferenceDeviceControl m_ActiveConferenceControl;

		public IConferenceDeviceControl ActiveConferenceControl
		{
			get { return m_ActiveConferenceControl; }
			set
			{
				if (value == m_ActiveConferenceControl)
					return;

				Unsubscribe(m_ActiveConferenceControl);
				m_ActiveConferenceControl = value;
				Subscribe(m_ActiveConferenceControl);
			}
		}

		protected AbstractConferencePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		#region Conference Control Callbacks
		
		protected virtual void Subscribe(IConferenceDeviceControl control)
		{
		}

		protected virtual void Unsubscribe(IConferenceDeviceControl control)
		{
		}

		#endregion
	}
}
