using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Displays;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Displays;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Displays
{
	public sealed class MenuCombinedAdvancedModePresenter : AbstractUiPresenter<IMenuCombinedAdvancedModeView>, IMenuCombinedAdvancedModePresenter
	{
		private readonly SafeCriticalSection m_RefreshSection;
		private readonly ReferencedAdvancedDisplayPresenterFactory m_PresenterFactory;

		public MenuCombinedAdvancedModePresenter(IConnectProNavigationController nav, IUiViewFactory views, ConnectProTheme theme) : base(nav, views, theme)
		{
			m_RefreshSection = new SafeCriticalSection();
			m_PresenterFactory = new ReferencedAdvancedDisplayPresenterFactory(nav, ItemFactory, Subscribe, Unsubscribe);
		}

		protected override void Refresh(IMenuCombinedAdvancedModeView view)
		{
			base.Refresh(view);

			m_RefreshSection.Enter();
			try
			{
				// todo 
				m_PresenterFactory.BuildChildren(null);
			}
			finally
			{
				m_RefreshSection.Leave();
			}
		}

		private IEnumerable<IReferencedAdvancedDisplayView> ItemFactory(ushort count)
		{
			return GetView().GetChildComponentViews(ViewFactory, count);
		}

		#region View Callbacks

		protected override void Subscribe(IMenuCombinedAdvancedModeView view)
		{
			base.Subscribe(view);
			
			view.OnSimpleModeButtonPressed += ViewOnSimpleModeButtonPressed;
			view.OnRouteSummaryButtonPressed += ViewOnRouteSummaryButtonPressed;
		}

		protected override void Unsubscribe(IMenuCombinedAdvancedModeView view)
		{
			base.Unsubscribe(view);
			
			view.OnSimpleModeButtonPressed -= ViewOnSimpleModeButtonPressed;
			view.OnRouteSummaryButtonPressed -= ViewOnRouteSummaryButtonPressed;
		}

		private void ViewOnSimpleModeButtonPressed(object sender, EventArgs e)
		{
			Navigation.NavigateTo<IMenuCombinedSimpleModePresenter>();
		}

		private void ViewOnRouteSummaryButtonPressed(object sender, EventArgs e)
		{
			Navigation.NavigateTo<MenuRouteSummaryPresenter>();
		}

		#endregion

		#region Child Callbacks

		private void Subscribe(IReferencedAdvancedDisplayPresenter presenter)
		{
			throw new NotImplementedException();
		}

		private void Unsubscribe(IReferencedAdvancedDisplayPresenter presenter)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
