#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IPresenters.Common.Settings.About;
using ICD.Profound.ConnectPRO.Themes.UserInterface.IViews.Common.Settings.About;

namespace ICD.Profound.ConnectPRO.Themes.UserInterface.Presenters.Common.Settings.About
{
	public sealed class ReferencedSettingsPluginsPresenterFactory :
		AbstractUiListItemFactory<Assembly, IReferencedSettingsPluginsPresenter, IReferencedSettingsPluginsView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		public ReferencedSettingsPluginsPresenterFactory(IConnectProNavigationController navigationController,
		                                                 ListItemFactory<IReferencedSettingsPluginsView> viewFactory)
			: base(navigationController, viewFactory, p => { }, p => { })
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(Assembly model, IReferencedSettingsPluginsPresenter presenter,
		                                     IReferencedSettingsPluginsView view)
		{
			presenter.Assembly = model;
			presenter.SetView(view);
		}
	}
}
