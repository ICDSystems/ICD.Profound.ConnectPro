using ICD.Connect.Conferencing.Controls.Dialing;
using ICD.Connect.Devices.Controls;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.Conference;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.Conference;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.Presenters.Conference
{
	[PresenterBinding(typeof(IConferenceBasePresenter))]
	public sealed class ConferenceBasePresenter : AbstractPopupPresenter<IConferenceBaseView>, IConferenceBasePresenter
	{
		public IConferenceDeviceControl ActiveConferenceControl { get; private set; }

		public ConferenceBasePresenter(ITouchDisplayNavigationController nav, ITouchDisplayViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		public void SetControl(IDeviceControl control)
		{
			IConferenceDeviceControl conferenceControl = control as IConferenceDeviceControl;
			if (conferenceControl == null)
				return;

			ActiveConferenceControl = conferenceControl;
		}

		public bool SupportsControl(IDeviceControl control)
		{
			return control is IConferenceDeviceControl;
		}
	}
}
