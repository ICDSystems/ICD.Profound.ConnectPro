using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;

namespace ICD.Profound.TouchCUE.Themes.TouchDisplayInterface.IViews.DeviceDrawer
{
    public interface IDeviceDrawerView : ITouchDisplayView
    {
	    event EventHandler<UShortEventArgs> OnAppButtonPressed;

	    IEnumerable<IReferencedSourceView> GetChildComponentViews(ITouchDisplayViewFactory views, ushort count);

	    void SetAppButtonIcons(IEnumerable<string> packageNames);
		
	    void SetAppButtonLabels(IEnumerable<string> appNames);
    }
}
