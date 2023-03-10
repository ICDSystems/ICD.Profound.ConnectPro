using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.UI.Attributes;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPROCommon.Themes;
using ICD.Profound.ConnectPROCommon.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.WebConferencing
{
	[PresenterBinding(typeof(IReferencedWebConferencingAlertPresenter))]
	public sealed class ReferencedWebConferencingAlertPresenter :
		AbstractUiComponentPresenter<IReferencedWebConferencingAlertView>, IReferencedWebConferencingAlertPresenter
	{
		public event EventHandler OnPressed;

		private readonly SafeCriticalSection m_RefreshSection;

		private WebConferencingAppInstructions m_App;

		[CanBeNull]
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
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public ReferencedWebConferencingAlertPresenter(IConnectProNavigationController nav, IUiViewFactory views, IConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			OnPressed = null;

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IReferencedWebConferencingAlertView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				string label = m_App == null ? null : m_App.Name;
				string url = m_App == null ? null : m_App.Icon;

				view.SetLabel(label);
				view.SetIconUrl(url);
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
		protected override void Subscribe(IReferencedWebConferencingAlertView view)
		{
			base.Subscribe(view);

			view.OnButtonPressed += ViewOnButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IReferencedWebConferencingAlertView view)
		{
			base.Unsubscribe(view);

			view.OnButtonPressed -= ViewOnButtonPressed;
		}

		private void ViewOnButtonPressed(object sender, EventArgs eventArgs)
		{
			OnPressed.Raise(this);
		}

		#endregion
	}
}
