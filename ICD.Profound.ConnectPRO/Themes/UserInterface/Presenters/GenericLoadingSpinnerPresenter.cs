using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Timers;
using ICD.Connect.UI.Attributes;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters
{
	[PresenterBinding(typeof(IGenericLoadingSpinnerPresenter))]
	public sealed class GenericLoadingSpinnerPresenter : AbstractUiPresenter<IGenericLoadingSpinnerView>, IGenericLoadingSpinnerPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly SafeTimer m_LoadingTimeOut;

		private string LoadingText { get; set; }

		public GenericLoadingSpinnerPresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_LoadingTimeOut = SafeTimer.Stopped(TimeOut);
		}

		protected override void Refresh(IGenericLoadingSpinnerView view)
		{
			m_RefreshSection.Execute(() => view.SetText(LoadingText));
		}

		/// <summary>
		/// Shows the view with the text parameter displayed.
		/// </summary>
		/// <param name="text"></param>
		public void ShowView(string text)
		{
			LoadingText = text;
			ShowView(true);
			RefreshIfVisible();
		}

		/// <summary>
		/// Shows the view with the text parameter displayed, and times out after timeoutMilliseconds.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="timeoutMilliseconds"></param>
		public void ShowView(string text, long timeoutMilliseconds)
		{
			ShowView(text);

			m_LoadingTimeOut.Reset(timeoutMilliseconds);
		}

		/// <summary>
		/// Time out the spinner with a generic message if the task takes too long.
		/// </summary>
		public void TimeOut()
		{
			TimeOut("Operation failed to complete.");
		}

		/// <summary>
		/// Time out the spinner with a specific message if the task takes too long.
		/// </summary>
		/// <param name="text"></param>
		public void TimeOut(string text)
		{
			ShowView(false);
			Navigation.LazyLoadPresenter<IGenericAlertPresenter>().Show(text);
		}

		/// <summary>
		/// When the view visibility changes stop the timeout timer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			m_LoadingTimeOut.Stop();
		}
	}
}
