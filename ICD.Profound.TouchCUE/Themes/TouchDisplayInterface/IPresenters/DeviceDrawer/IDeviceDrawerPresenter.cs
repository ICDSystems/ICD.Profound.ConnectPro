using System;
using System.Collections.Generic;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Profound.ConnectPROCommon.Routing;
using ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer
{
    public interface IDeviceDrawerPresenter : ITouchDisplayPresenter<IDeviceDrawerView>, IMainPagePresenter
    {
	    event EventHandler<SourceEventArgs> OnSourcePressed;

	    void SetRoutedSources(Dictionary<ISource, eSourceState> sources);
    }
}
