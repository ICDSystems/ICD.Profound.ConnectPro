using System;
using System.Collections.Generic;
using ICD.Connect.Routing.Endpoints.Sources;
using ICD.Connect.Routing.EventArguments;
using ICD.Profound.ConnectPRO.Routing;
using ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.DeviceDrawer;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IPresenters.DeviceDrawer
{
    public interface IDeviceDrawerPresenter : ITouchDisplayPresenter<IDeviceDrawerView>
    {
	    event EventHandler<SourceEventArgs> OnSourcePressed;

	    void SetRoutedSources(Dictionary<ISource, eSourceState> sources);
    }
}
