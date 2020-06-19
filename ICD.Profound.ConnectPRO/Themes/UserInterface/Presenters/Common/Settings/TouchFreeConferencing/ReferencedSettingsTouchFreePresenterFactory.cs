using System;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.TouchFreeConferencing;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.TouchFreeConferencing;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.TouchFreeConferencing
{
	public sealed class ReferencedSettingsTouchFreePresenterFactory : AbstractUiListItemFactory<ISource, IReferencedSettingsTouchFreePresenter, IReferencedSettingsTouchFreeView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedSettingsTouchFreePresenterFactory(IConnectProNavigationController navigationController,
		                                              ListItemFactory<IReferencedSettingsTouchFreeView> viewFactory,
		                                              Action<IReferencedSettingsTouchFreePresenter> subscribe,
													  Action<IReferencedSettingsTouchFreePresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(ISource model, IReferencedSettingsTouchFreePresenter presenter,
											 IReferencedSettingsTouchFreeView view)
		{
			presenter.Source = model;
			presenter.SetView(view);
		}
	}
}