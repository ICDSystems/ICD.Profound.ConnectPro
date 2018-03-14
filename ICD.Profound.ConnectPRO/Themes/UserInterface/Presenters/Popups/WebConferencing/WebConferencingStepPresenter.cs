using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.WebConferencing
{
	public sealed class WebConferencingStepPresenter : AbstractPresenter<IWebConferencingStepView>, IWebConferencingStepPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;

		private WebConferencingAppInstructions m_App;
		private int m_StepNumber;

		public WebConferencingAppInstructions App
		{
			get { return m_App; }
			set
			{
				if (value == m_App)
					return;
				
				m_App = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Which step to show, starting at 1.
		/// </summary>
		public int StepNumber
		{
			get { return m_StepNumber; }
			set
			{
				int count = App == null ? 0 : App.Count;
				value = MathUtils.Clamp(value, 1, count);

				if (value == m_StepNumber)
					return;

				m_StepNumber = value;

				RefreshIfVisible();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WebConferencingStepPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWebConferencingStepView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				WebConferencingStepInstructions step =
					App == null && StepNumber >= 1
						? null
						: App.ElementAt(StepNumber - 1);

				string url = step == null ? null : step.Image;
				ushort number = (ushort)StepNumber;
				string text = step == null ? null : step.Text;
				bool back = StepNumber > 1;
				bool forward = App != null && StepNumber < App.Count;

				view.SetImageUrl(url);
				view.SetStepNumber(number);
				view.SetText(text);
				view.ShowBackButton(back);
				view.ShowForwardButton(forward);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWebConferencingStepView view)
		{
			base.Subscribe(view);

			view.OnCloseButtonPressed += ViewOnCloseButtonPressed;
			view.OnBackButtonPressed += ViewOnBackButtonPressed;
			view.OnForwardButtonPressed += ViewOnForwardButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWebConferencingStepView view)
		{
			base.Unsubscribe(view);

			view.OnCloseButtonPressed -= ViewOnCloseButtonPressed;
			view.OnBackButtonPressed -= ViewOnBackButtonPressed;
			view.OnForwardButtonPressed -= ViewOnForwardButtonPressed;
		}

		private void ViewOnForwardButtonPressed(object sender, EventArgs eventArgs)
		{
			StepNumber++;
		}

		private void ViewOnBackButtonPressed(object sender, EventArgs eventArgs)
		{
			StepNumber--;
		}

		private void ViewOnCloseButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		/// <summary>
		/// Called when the view visibility changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void ViewOnVisibilityChanged(object sender, BoolEventArgs args)
		{
			base.ViewOnVisibilityChanged(sender, args);

			StepNumber = 1;
		}

		#endregion
	}
}
