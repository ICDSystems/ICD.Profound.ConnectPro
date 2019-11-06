using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(ICallOutWtcReferencedLeftMenuPresenter))]
	public sealed class CallOutWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                            ICallOutWtcReferencedLeftMenuPresenter
	{
		private readonly IWtcCallOutPresenter m_CallOutPresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public CallOutWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views,
		                                             ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_CallOutPresenter = nav.LazyLoadPresenter<IWtcCallOutPresenter>();
			Subscribe(m_CallOutPresenter);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			Icon = "call";
			Enabled = true;
			Label = "Call Out";
			State = null;
			Selected = m_CallOutPresenter.IsViewVisible;

			base.Refresh(view);
		}

		public override void HideSubpages()
		{
			m_CallOutPresenter.ShowView(false);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_CallOutPresenter);
		}

		#region Presenter Callbacks

		private void Subscribe(IWtcCallOutPresenter callOutPresenter)
		{
			callOutPresenter.OnViewVisibilityChanged += CallOutPresenterOnViewVisibilityChanged;
		}

		private void Unsubscribe(IWtcCallOutPresenter callOutPresenter)
		{
			callOutPresenter.OnViewVisibilityChanged -= CallOutPresenterOnViewVisibilityChanged;
		}

		private void CallOutPresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			Navigation.NavigateTo<IWtcCallOutPresenter>();
		}
	}
}
