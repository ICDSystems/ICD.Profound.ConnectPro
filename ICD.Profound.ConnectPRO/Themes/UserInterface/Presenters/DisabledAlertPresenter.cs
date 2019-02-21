using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	[PresenterBinding(typeof(IDisabledAlertPresenter))]
	public sealed class DisabledAlertPresenter : AbstractUiPresenter<IDisabledAlertView>, IDisabledAlertPresenter
	{
		private readonly SafeTimer m_HideTimer;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public DisabledAlertPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_HideTimer = SafeTimer.Stopped(() => ShowView(false));
		}

		protected override void Subscribe(IDisabledAlertView view)
		{
			base.Subscribe(view);
			view.OnDismissButtonPressed += View_OnDismissButtonPressed;
		}

		private void View_OnDismissButtonPressed(object sender, System.EventArgs e)
		{
			m_HideTimer.Stop();
			ShowView(false);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			m_HideTimer.Dispose();
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_HideTimer.Reset(2 * 1000);
		}
	}
}