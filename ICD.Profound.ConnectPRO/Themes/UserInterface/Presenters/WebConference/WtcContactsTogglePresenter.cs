using System;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference
{
	public class WtcContactsTogglePresenter : AbstractPresenter<IWtcContactsToggleView>, IWtcContactsTogglePresenter
	{
		public WtcContactsTogglePresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
		}

		protected override void Refresh(IWtcContactsToggleView view)
		{
			base.Refresh(view);

			view.SetContactsMode(false);
			view.SetButtonVisible(true);
		}

		#region View Callbacks

		protected override void Subscribe(IWtcContactsToggleView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnOnButtonPressed;
		}

		protected override void Unsubscribe(IWtcContactsToggleView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnOnButtonPressed;
		}

		private void ViewOnOnButtonPressed(object sender, EventArgs e)
		{
			Navigation.NavigateTo<IWtcMainPagePresenter>();
		}

		#endregion
	}
}