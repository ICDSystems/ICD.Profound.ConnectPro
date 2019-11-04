using System;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(ICallOutWtcReferencedLeftMenuPresenter))]
	public sealed class CallOutWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter,
	                                                            ICallOutWtcReferencedLeftMenuPresenter
	{
		private IWtcCallOutPresenter m_CallOutPresenter;

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

			Icon = "call";
			Enabled = true;
			Label = "Call Out";
			State = null;
		}

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
			Selected = m_CallOutPresenter.IsViewVisible;
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
