using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.DeviceDrawer
{
    public interface IDeviceDrawerView : ITouchDisplayView
    {
	    event EventHandler<UShortEventArgs> OnAppButtonPressed;

	    IEnumerable<IReferencedSourceView> GetChildComponentViews(ITouchDisplayViewFactory views, ushort count);

	    void SetAppButtonIcons(IEnumerable<string> packageNames);
    }
}
