using System;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public class WtcActiveMeetingTogglePresenter : AbstractUiPresenter<IWtcActiveMeetingToggleView>, IWtcActiveMeetingTogglePresenter
	{
		public WtcActiveMeetingTogglePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
		}

		protected override void Refresh(IWtcActiveMeetingToggleView view)
		{
			base.Refresh(view);

			view.SetActiveMeetingMode(false);
			view.SetButtonVisible(true);
		}

		#region View Callbacks

		protected override void Subscribe(IWtcActiveMeetingToggleView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnOnButtonPressed;
		}

		protected override void Unsubscribe(IWtcActiveMeetingToggleView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnOnButtonPressed;
		}

		private void ViewOnOnButtonPressed(object sender, EventArgs e)
		{
			Navigation.NavigateTo<IWtcButtonListPresenter>();
		}

		#endregion
	}
}