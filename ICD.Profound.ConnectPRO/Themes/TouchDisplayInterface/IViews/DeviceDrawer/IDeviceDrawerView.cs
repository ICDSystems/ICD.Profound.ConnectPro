using System.Collections.Generic;

namespace ICD.Profound.ConnectPRO.Themes.TouchDisplayInterface.IViews.DeviceDrawer
{
    public interface IDeviceDrawerView : ITouchDisplayView
    {
	    IEnumerable<IReferencedSourceView> GetChildComponentViews(ITouchDisplayViewFactory views, ushort count);
    }
}
