using ICD.Common.Utils.EventArguments;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.WebConference.LeftMenu.Buttons;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.WebConference.LeftMenu;
using ICD.Profound.ConnectPROCommon.Themes;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.WebConference.LeftMenu.Buttons
{
	[PresenterBinding(typeof(IShareWtcReferencedLeftMenuPresenter))]
	public sealed class ShareWtcReferencedLeftMenuPresenter : AbstractWtcReferencedLeftMenuPresenter, IShareWtcReferencedLeftMenuPresenter
	{
		private readonly IWtcSharePresenter m_SharePresenter;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ShareWtcReferencedLeftMenuPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_SharePresenter = nav.LazyLoadPresenter<IWtcSharePresenter>();
			Subscribe(m_SharePresenter);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Unsubscribe(m_SharePresenter);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWtcReferencedLeftMenuView view)
		{
			Label = "Share";
			Enabled = true;
			Icon = "exit";
			Selected = m_SharePresenter.IsViewVisible;

			base.Refresh(view);
		}

		public override void HideSubpages()
		{
			m_SharePresenter.ShowView(false);
		}

		/// <summary>
		/// Override to handle what happens when the button is pressed.
		/// </summary>
		protected override void HandleButtonPress()
		{
			m_SharePresenter.ShowView(true);
		}

		#region Share Presenter Callbacks

		/// <summary>
		/// Subscribe to the share presenter events.
		/// </summary>
		/// <param name="sharePresenter"></param>
		private void Subscribe(IWtcSharePresenter sharePresenter)
		{
			sharePresenter.OnViewVisibilityChanged += SharePresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Unsubscribe from the share presenter events.
		/// </summary>
		/// <param name="sharePresenter"></param>
		private void Unsubscribe(IWtcSharePresenter sharePresenter)
		{
			sharePresenter.OnViewVisibilityChanged -= SharePresenterOnViewVisibilityChanged;
		}

		/// <summary>
		/// Called when the share presenter visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SharePresenterOnViewVisibilityChanged(object sender, BoolEventArgs e)
		{
			RefreshIfVisible();
		}

		#endregion
	}
}
