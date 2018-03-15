using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Popups.WebConferencing;
using ICD.Profound.ConnectPRO.WebConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Popups.WebConferencing
{
	public sealed class WebConferencingAlertPresenter : AbstractPresenter<IWebConferencingAlertView>, IWebConferencingAlertPresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedWebConferencingAlertPresenterFactory m_ChildrenFactory;

		private WebConferencingAppInstructions[] m_Apps;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="views"></param>
		/// <param name="theme"></param>
		public WebConferencingAlertPresenter(INavigationController nav, IViewFactory views, ConnectProTheme theme)
			: base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_ChildrenFactory = new ReferencedWebConferencingAlertPresenterFactory(nav, ItemFactory);

			m_Apps = new WebConferencingAppInstructions[0];
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public override void Dispose()
		{
			UnsubscribeChildren();
			m_ChildrenFactory.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		/// <param name="view"></param>
		protected override void Refresh(IWebConferencingAlertView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();

			try
			{
				UnsubscribeChildren();

				m_Apps = Theme.WebConferencingInstructions.ToArray();

				foreach (IReferencedWebConferencingAlertPresenter presenter in m_ChildrenFactory.BuildChildren(m_Apps))
				{
					Subscribe(presenter);
					presenter.ShowView(true);
				}

				view.SetAppCount((ushort)m_Apps.Length);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		/// <summary>
		/// Unsubscribes from all of the child presenters.
		/// </summary>
		private void UnsubscribeChildren()
		{
			foreach (IReferencedWebConferencingAlertPresenter presenter in m_ChildrenFactory)
				Unsubscribe(presenter);
		}

		private IEnumerable<IReferencedWebConferencingAlertView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#region View Callbacks

		/// <summary>
		/// Subscribe to the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Subscribe(IWebConferencingAlertView view)
		{
			base.Subscribe(view);

			view.OnDismissButtonPressed += ViewOnDismissButtonPressed;
		}

		/// <summary>
		/// Unsubscribe from the view events.
		/// </summary>
		/// <param name="view"></param>
		protected override void Unsubscribe(IWebConferencingAlertView view)
		{
			base.Unsubscribe(view);

			view.OnDismissButtonPressed -= ViewOnDismissButtonPressed;
		}

		private void ViewOnDismissButtonPressed(object sender, EventArgs eventArgs)
		{
			ShowView(false);
		}

		#endregion

		#region Child Callbacks

		/// <summary>
		/// Subscribe to the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Subscribe(IReferencedWebConferencingAlertPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed += ChildOnPressed;
		}

		/// <summary>
		/// Unsubscribe from the child events.
		/// </summary>
		/// <param name="child"></param>
		private void Unsubscribe(IReferencedWebConferencingAlertPresenter child)
		{
			if (child == null)
				return;

			child.OnPressed -= ChildOnPressed;
		}

		/// <summary>
		/// Called when the user presses the child source.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildOnPressed(object sender, EventArgs eventArgs)
		{
			IReferencedWebConferencingAlertPresenter child = sender as IReferencedWebConferencingAlertPresenter;
			if (child == null)
				return;

			Navigation.NavigateTo<IWebConferencingStepPresenter>().App = child.App;
		}

		#endregion
	}
}
