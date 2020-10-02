using System;
using ICD.Connect.Devices;
using ICD.Connect.UI.Mvp.Presenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IPresenters.Headers;
using ICD.Profound.ConnectPROCommon.Themes.OsdInterface.IViews.Headers;

namespace ICD.Profound.ConnectPROCommon.Themes.OsdInterface.Presenters.Headers
{
	public sealed class ReferencedCriticalDevicePresenterFactory : AbstractOsdListItemFactory<IDevice,
		IReferencedCriticalDevicePresenter, IReferencedCriticalDeviceView>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="navigationController"></param>
		/// <param name="viewFactory"></param>
		/// <param name="subscribe"></param>
		/// <param name="unsubscribe"></param>
		public ReferencedCriticalDevicePresenterFactory(IOsdNavigationController navigationController,
		                                                ListItemFactory<IReferencedCriticalDeviceView> viewFactory,
		                                                Action<IReferencedCriticalDevicePresenter> subscribe,
		                                                Action<IReferencedCriticalDevicePresenter> unsubscribe)
			: base(navigationController, viewFactory, subscribe, unsubscribe)
		{
		}

		/// <summary>
		/// Binds the model and view to the presenter.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="presenter"></param>
		/// <param name="view"></param>
		protected override void BindMvpTriad(IDevice model, IReferencedCriticalDevicePresenter presenter, IReferencedCriticalDeviceView view)
		{
			presenter.Device = model;
			presenter.SetView(view);
		}
	}
}
